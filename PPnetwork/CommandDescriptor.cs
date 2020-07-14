using System;
using System.Reflection;

namespace PPnetwork
{
	/// <summary>
	/// Descriptor for Commands.
	/// In addition to Descriptor, ut also stores argument count and priority of the Command.
	/// </summary>
	/// <typeparam name="Application">Your specific Application implementation</typeparam>
	class CommandDescriptor<Application> : Descriptor<IApplication, ICommandArgument, Application>, ICommandDescriptor
		where Application : IApplication
	{
		readonly Type CommandArgumentType;
		public int ArgumentCount { get; }
		public int Priority { get; }

		/// <summary>
		/// Creates the Descriptor.
		/// </summary>
		public CommandDescriptor(Type commandArgumentType, int argumentCount, int priority = 0)
			: base(typeof(ICommandHandler<>), commandArgumentType)
		{
			CommandArgumentType = commandArgumentType;
			ArgumentCount = argumentCount;
			Priority = priority;
		}

		public void Invoke(IApplication application, object[]? parameters)
		{
			// CreateInstance throws exceptions packaged in a TargetInvocationException
			try
			{
				Invoke(application, (ICommandArgument)Activator.CreateInstance(CommandArgumentType, parameters)!);
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}
		}
	}

	/// <summary>
	/// <see cref="CommandDescriptor{Application}"/> version for when the type of Command Argument is known at compile time.
	/// </summary>
	/// <typeparam name="Application">Your specific Application.</typeparam>
	/// <typeparam name="CommandArgument">The Command Argument type.</typeparam>
	class CommandDescriptor<Application, CommandArgument> : CommandDescriptor<Application>
		where Application : IApplication
		where CommandArgument : ICommandArgument
	{
		// This version only makes sense when the Descriptor is used directly,
		// not through a Sniffer, therefore argument count and priority are irrelevant
		/// <summary>
		/// Creates the Descriptor with 0 argument count and 0 priority.
		/// </summary>
		public CommandDescriptor()
			: base(typeof(CommandArgument), 0, 0)
		{}
	}
}
