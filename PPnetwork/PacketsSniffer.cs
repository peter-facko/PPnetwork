using System;
using System.Collections.Generic;

namespace PPnetwork
{
	class PacketsSniffer<Connection> : Sniffer<Type, IInvoker<IConnection, IPacket>, Connection>
		where Connection : IConnection
	{
		public PacketsSniffer()
			: base(new Dictionary<Type, IInvoker<IConnection, IPacket>>(), typeof(IPacketHandler<>))
		{ }

		protected override void Handle(Type packetType)
			=> Add(packetType, new Descriptor<IConnection, IPacket, Connection>(typeof(IPacketHandler<>), packetType));
	}
}
