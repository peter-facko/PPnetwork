namespace PPnetwork
{
	/// <summary>
	/// Basic interface for a PPnetwork application.
	/// It can read and write <see cref="string"/> and remove a connection
	/// </summary>
	public interface IApplication : IReaderWriter<string>
	{
		void RemoveConnection(IConnection connection);
	}
}
