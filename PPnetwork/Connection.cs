using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace PPnetwork
{
	/// <summary>
	/// Basic implementation of <see cref="IConnection"/>. All other <see cref="IConnection"/>s should inherit this class.
	/// </summary>
	/// <typeparam name="SpecificApplication"></typeparam>
	/// <typeparam name="ApplicationConnection"></typeparam>
	public abstract class Connection<SpecificApplication, ApplicationConnection> : IConnection,
		IPacketHandler<EndPacket>

		where SpecificApplication : IApplication
		where ApplicationConnection : IConnection
	{
		/// <summary>
		/// Parent application.
		/// </summary>
		public SpecificApplication Application { get; }
		readonly TcpClient tcpClient;

		/// <summary>
		/// Stream that reads and writes packets into the connection.
		/// </summary>
		public IReaderWriter<IPacket> Stream { get; }
		readonly Thread thread;
		bool should_close = false;

		static readonly IInvoker<IConnection, IPacket> Parser = new PacketParser<ApplicationConnection>();

		/// <summary>
		/// Constructs a connection with specified parent application and a connected <see cref="TcpClient"/>. Throws if <paramref name="tcpClient"/> is not connected.
		/// </summary>
		/// <exception cref="Exception"/>
		protected Connection(SpecificApplication application, TcpClient tcpClient)
		{
			if (!tcpClient.Connected)
				throw new Exception("The client provided to the constructor should be connected already.");

			Application = application;
			this.tcpClient = tcpClient;
			Stream = new FormatterStream<BinaryFormatter, IPacket>(tcpClient.GetStream());
			thread = new Thread(Handle);
			thread.Start();
		}

		/// <summary>
		/// Forcibly closes the connection. Send an <see cref="EndPacket"/> first if you want to notify the other side.
		/// </summary>
		public void Close()
		{
			lock (this)
			{
				should_close = true;
			}
			tcpClient.Close();
			thread.Join();
		}

		/// <summary>
		/// Handles incoming packets.
		/// </summary>
		void Handle()
		{
			try
			{
				while (true)
					Parser.Invoke(this, Stream.Read());
			}
			catch (EndException e)
			{
				HandleNormalConnectionClose(e.Reason);
				RemoveConnection();
			}
			catch
			{
				bool sc;
				lock (this)
				{
					sc = should_close;
				}
				if (!sc)
				{
					HandleAbruptConnectionClose();
					RemoveConnection();
				}
			}
		}

		/// <summary>
		/// Removes this connection from its parent application.
		/// The connection should be closed before removing.
		/// </summary>
		void RemoveConnection()
		{
			Application.RemoveConnection(this);
		}

		/// <summary>
		/// Handler for when a connection closes unexpectedly.
		/// </summary>
		public abstract void HandleAbruptConnectionClose();

		/// <summary>
		/// Handler for when a connection closes after first sending an <see cref="EndPacket"/>.
		/// </summary>
		public abstract void HandleNormalConnectionClose(string reason);

		public void Handle(EndPacket packet)
		{
			throw new EndException(packet.Reason);
		}
	}
}
