namespace PPnetwork
{
	/// <summary>
	/// Interface used by <see cref="IConnection"/>s to handle incoming packets.
	/// </summary>
	/// <typeparam name="Packet"></typeparam>
	public interface IPacketHandler<Packet>
		where Packet : IPacket
	{
		void Handle(Packet packet);
	}
}
