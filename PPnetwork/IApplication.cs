namespace PPnetwork
{
	/// <summary>
	/// Basic interface for a PPnetwork application.
	/// It can read and write <see cref="string"/>, remove a connection
	/// and handle an <see cref="IConnection"/> closing either normally or unexpectedly.
	/// </summary>
	public interface IApplication : IReaderWriter<string>
	{
		void HandleNormalConnectionClose(IConnection connection, string reason);
		void HandleAbruptConnectionClose(IConnection connection);
		void RemoveConnection(IConnection connection);
	}
}
