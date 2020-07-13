using System;
using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// Basic implementation of <see cref="IApplication"/>. All other <see cref="IApplication"/>s should inherit this class.
	/// </summary>
	/// <typeparam name="SpecificApplication"></typeparam>
	public abstract class Application<SpecificApplication> : IApplication,
		ICommandHandler<NotFoundCommandArgument>,
		ICommandHandler<BadArgumentCountCommandArgument>,
		ICommandHandler<ExitCommandArgument>

		where SpecificApplication : IApplication
	{
		/// <summary>
		/// Writes a line to the console.
		/// </summary>
		/// <param name="line">The line to write.</param>
		public void Write(string line)
		{
			Console.WriteLine(line);
		}
		/// <summary>
		/// Reads a line from the console.
		/// </summary>
		/// <returns>A line from the console</returns>
		public string Read()
		{
			return Console.ReadLine()!;
		}

		/// <summary>
		/// Writes to console if build in debug mode. Otherwise does nothing.
		/// </summary>
		/// <param name="s"></param>
		public void WriteDebug(string line)
		{
#if DEBUG
			Write(line);
#endif
		}
		/// <summary>
		/// All connections this application has.
		/// </summary>
		protected abstract IEnumerable<IConnection> Connections { get; }
		/// <summary>
		/// Removes all connections.
		/// All connections have to be closed before the call.
		/// </summary>
		protected abstract void ClearConnections();
		/// <summary>
		/// Removes the <paramref name="connection"/>.
		/// <paramref name="connection"/> must be closed.
		/// </summary>
		public abstract void RemoveConnection(IConnection connection);

		/// <summary>
		/// Handler after the exit command is entered and all connections are closed.
		/// </summary>
		protected virtual void HandleAfterExit()
		{ }

		/// <summary>
		/// Message to sent to the connections when exit command is entered.
		/// </summary>
		protected abstract string ExitMessage { get; }

		/// <summary>
		/// Closes all connections by sending an <see cref="EndPacket"/> and them forcibly closing.
		/// </summary>
		/// <param name="reason">Reason for closing the connections. This is sent in the packet.</param>
		protected void CloseAllConnections(string reason)
		{
			foreach (var connection in Connections)
				connection.Stream.Write(new EndPacket(reason));			

			foreach (var connection in Connections)
				connection.Close();

			ClearConnections();
		}

		static readonly IInvoker<IApplication, string> Parser = new CommandParser<SpecificApplication>();

		/// <summary>
		/// Parses and handles commands from input.
		/// </summary>
		public void AcceptCommands()
		{
			string s;

			while (true)
			{
				try
				{
					s = Read().Trim();
					if (s != "")
						Parser.Invoke(this, s);
				}
				catch (ExitCommandException)
				{
					CloseAllConnections(ExitMessage);
					HandleAfterExit();
					break;
				}
				catch (ParseException pe)
				{
					Write(pe.Message);
				}
			}
		}

		public void Handle(BadArgumentCountCommandArgument argument)
		{
			Write($"bad argument count: {argument.ArgumentCount}");
		}

		public void Handle(ExitCommandArgument _)
		{
			throw new ExitCommandException();
		}

		/// <summary>
		/// Handles invalid input.
		/// </summary>
		public abstract void Handle(NotFoundCommandArgument argument);
	}
}
