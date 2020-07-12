namespace PPnetwork
{
	/// <summary>
	/// A class which can read and write data of type <typeparamref name="T"/>.
	/// </summary>
	public interface IReaderWriter<T> : IReader<T>, IWriter<T>
	{}
}
