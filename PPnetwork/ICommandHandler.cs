namespace PPnetwork
{
	/// <summary>
	/// Interface used by <see cref="IApplication"/>s to handle commands.
	/// </summary>
	/// <typeparam name="CommandArgument"></typeparam>
	public interface ICommandHandler<CommandArgument>
		where CommandArgument : ICommandArgument
	{
		void Handle(CommandArgument argument);
	}
}
