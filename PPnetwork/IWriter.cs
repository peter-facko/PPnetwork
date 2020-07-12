namespace PPnetwork
{
	/// <summary>
	/// A class which can write data of type <typeparamref name="T"/>.
	/// </summary>
	public interface IWriter<in T>
	{
		void Write(T t);
	}
}
