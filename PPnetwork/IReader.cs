namespace PPnetwork
{
	/// <summary>
	/// A class which can read data of type <typeparamref name="T"/>.
	/// </summary>
	public interface IReader<out T>
	{
		T Read();
	}
}
