namespace PPnetwork
{
	/// <summary>
	/// A class which can invoke a method on a caller with parameters.
	/// </summary>
	public interface IInvoker<in Caller, in Parameter>
	{
		void Invoke(Caller caller, Parameter parameter);
	}
}
