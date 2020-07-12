namespace PPnetwork
{
	/// <summary>
	/// <see cref="ICommandArgument"/> representing a bad argument count in input. Used only internally by the library
	/// </summary>
	public readonly struct BadArgumentCountCommandArgument : ICommandArgument
	{
		/// <summary>
		/// The number of arguments passed in input.
		/// </summary>
		public readonly int ArgumentCount;

		public BadArgumentCountCommandArgument(int argumentCount)
		{
			ArgumentCount = argumentCount;
		}
	}
}
