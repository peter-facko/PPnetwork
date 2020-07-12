namespace PPnetwork
{
	/// <summary>
	/// Basic interface for a network connection.
	/// It can send and receive packets and close.
	/// </summary>
	public interface IConnection
	{
		IReaderWriter<IPacket> Stream { get; }
		/// <summary>
		/// Forcibly closes the connection.
		/// </summary>
		void Close();
	}
}
