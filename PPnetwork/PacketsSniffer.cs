using System;
using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// A small extension to the Sniffer base.
	/// </summary>
	/// <typeparam name="Connection"></typeparam>
	class PacketsSniffer<Connection> : Sniffer<Type, IInvoker<IConnection, IPacket>, Connection>
		where Connection : IConnection
	{
		public PacketsSniffer()
			: base(new Dictionary<Type, IInvoker<IConnection, IPacket>>(), typeof(IPacketHandler<>))
		{ }

		/// <summary>
		/// Just creates the right Descriptor and adds it to the Sniffer cache
		/// </summary>
		protected override void Handle(Type packetType)
			=> Add(packetType, new Descriptor<IConnection, IPacket, Connection>(typeof(IPacketHandler<>), packetType));
	}
}
