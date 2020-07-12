namespace PPnetwork
{
	public interface IInvoker<in Caller, in Parameter>
	{
		void Invoke(Caller caller, Parameter parameter);
	}
}
