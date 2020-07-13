namespace PPnetwork
{
	/// <summary>
	/// Basic interface for a PPnetwork application.
	/// It can read and write <see cref="string"/>, remove a connection
	/// and handle an <see cref="IConnection"/> closing either normally or unexpectedly.
	/// </summary>
	public interface IApplication : IReaderWriter<string>
	{
		void RemoveConnection(IConnection connection);
	}
}
