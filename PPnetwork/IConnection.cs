namespace PPnetwork
{
	public interface IConnection
	{
		IReaderWriter<IPacket> Stream { get; }
		void Close();
	}
}
