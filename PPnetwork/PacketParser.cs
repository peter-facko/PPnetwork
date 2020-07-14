using System;

namespace PPnetwork
{
	/// <summary>
	/// As packets don't require parsing,
	/// this just encapsulates the <see cref="PacketsSniffer{Connection}"/>
	/// and pulls Descriptors out of it
	/// </summary>
	/// <typeparam name="Connection"></typeparam>
	class PacketParser<Connection> : IParser<IConnection, IPacket, IPacket>
		where Connection : IConnection
	{
		// Although PacketsSniffer returns whole Descriptors,
		// Parser is only interested in their Invoker capabilities

		static readonly ISimpleReadonlyDictionary<Type, IInvoker<IConnection, IPacket>> packetInfo = new PacketsSniffer<Connection>();

		public (IInvoker<IConnection, IPacket>, IPacket) Parse(IPacket input)
			=> (packetInfo.GetValue(input.GetType())!, input);
	}
}
