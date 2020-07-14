using System;
using System.Collections.Generic;
using System.Reflection;

namespace PPnetwork
{
	class CommandsSniffer<Application> : Sniffer<ReadOnlyMemory<char>, ICommandArgumentCountDictionary, Application>, ICommandsSniffer
		where Application : IApplication
	{
		public ICommandDescriptor NotFoundCommand { get; }
		public ICommandDescriptor BadArgumentCountCommand { get; }

		public CommandsSniffer()
			: base(new SortedDictionary<ReadOnlyMemory<char>, ICommandArgumentCountDictionary>(new MemoryStringComparer()), typeof(ICommandHandler<>))
		{
			NotFoundCommand = new CommandDescriptor<Application, NotFoundCommandArgument>();
			BadArgumentCountCommand = new CommandDescriptor<Application, BadArgumentCountCommandArgument>();
		}

		protected override void Handle(Type orderType)
		{
			// The Command Argument types will be found by the base Sniffer,
			// but are called directly, so no need to register them in the cache
			if (orderType == typeof(NotFoundCommandArgument) ||
				orderType == typeof(BadArgumentCountCommandArgument))
				return;

			var attribute = orderType.GetCustomAttribute<CommandAttribute>()!;
			var argumentCount = attribute.HasOneLongArgument ? 0 : orderType.GetFields().Length;
			ICommandDescriptor command = new CommandDescriptor<Application>(orderType, argumentCount, attribute.Priority);

			// This is a very long section doing the optimizations allowed by CommandFlags
			// For example, if a Command has a unique name,
			// there is no need to store a whole collection of commands with such name that can have different argument counts,
			// because the sniffer is told that there are none.
			// All classes ending with CommandArgumentCountDictionary handle these special cases.

			if (attribute.HasUniqueName)
			{
				ICommandArgumentCountDictionary commands;
				if (attribute.HasOneLongArgument)
					commands = new UniqueNameAndOneLongArgumentCommandArgumentCountDictionary(command);
				else
					commands = new UniqueNameCommandArgumentCountDictionary(command);
				Add(attribute.Name.AsMemory(), commands);
			}
			else
			{
				var commands = GetValue(attribute.Name.AsMemory());
				if (commands == null)
				{
					if (attribute.HasOneLongArgument)
						commands = new OneLongArgumentCommandArgumentCountDictionary();
					else
						commands = new BasicCommandArgumentCountDictionary();
					Add(attribute.Name.AsMemory(), commands);
				}

				if (attribute.HasOneLongArgument)
					commands.AddIfOneLongArgument(command);
				else
				{
					if (attribute.HasUniqueArgumentCount)
						commands.Add(argumentCount, new FakeCollection<ICommandDescriptor>(command.AsSingleEnumerable()));
					else
					{
						var commandsWithSameArgumentCount = commands.GetValue(argumentCount);
						if (commandsWithSameArgumentCount == null)
						{
							commandsWithSameArgumentCount = new SimpleCollection<SortedSet<ICommandDescriptor>, ICommandDescriptor> { command };
							commands.Add(argumentCount, commandsWithSameArgumentCount);
						}
						else
							commandsWithSameArgumentCount.Add(command);
					}
				}
			}
		}

		// Because C# doesn't have return type covariance.
		ICommandArgumentCountReadonlyDictionary? ISimpleReadonlyDictionary<ReadOnlyMemory<char>, ICommandArgumentCountReadonlyDictionary>.GetValue(ReadOnlyMemory<char> from)
			=> GetValue(from);
	}
}
