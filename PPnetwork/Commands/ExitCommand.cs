namespace PPnetwork
{
	/// <summary>
	/// <see cref="ICommandArgument"/> used to exit application. Used only internally by the library.
	/// </summary>
	[Command("exit", CommandFlags.UniqueName)]
	public struct ExitCommandArgument : ICommandArgument
	{}
}
