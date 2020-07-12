namespace PPnetwork
{
	public interface IWriter<in T>
	{
		void Write(T t);
	}
}
