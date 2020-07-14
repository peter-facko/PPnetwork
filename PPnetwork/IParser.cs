namespace PPnetwork
{
	/// <summary>
	/// A more sophisticated <see cref="IInvoker{,}"/>,
	/// that can also parse an input into an invoker and a parameter for it.
	/// </summary>
	interface IParser<Caller, Input, Parameter> : IInvoker<Caller, Input>
	{
		(IInvoker<Caller, Parameter>, Parameter) Parse(Input input);

		/// <summary>
		/// Parses the <paramref name="input"/>, which creates an Invoker and parameters
		/// and invokes this Invoker with <paramref name="caller"/> and these parameters.
		/// </summary>
		/// <param name="caller"></param>
		/// <param name="input"></param>
		void IInvoker<Caller, Input>.Invoke(Caller caller, Input input)
		{
			var (invoker, parameter) = Parse(input);
			invoker.Invoke(caller, parameter);
		}
	}
}
