namespace PPnetwork
{
	public interface IReader<out T>
	{
		T Read();
	}
}
