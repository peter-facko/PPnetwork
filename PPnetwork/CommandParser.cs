using System;
using System.Collections.Generic;

namespace PPnetwork
{
	using InvokersParametersPair = ValueTuple<IEnumerable<IInvoker<IApplication, object[]>>, object[]>;

	class CommandParser<Application> : IParser<IApplication, string, object[]>
		where Application : IApplication
	{
		static readonly ICommandsSniffer commandsSniffer = new CommandsSniffer<Application>();

		// Assuming that Commands are handled on one thread only.
		// We can assume this because this class is internal
		// the library parses Commands on one thread.
		static readonly object[] arrayHelper = new object[1];

		// Helper for one argument actions.
		static InvokersParametersPair JustOneArgument(IEnumerable<IInvoker<IApplication, object[]>> commands, object o)
		{
			arrayHelper[0] = o;
			return (commands, arrayHelper);
		}

		// Helper for one argument and one command actions.
		static InvokersParametersPair JustOneArgument(ICommandDescriptor command, object o)
			=> JustOneArgument(command.AsSingleEnumerable(), o);
		
		// This returns an IEnumerable.
		static InvokersParametersPair ParseImplementation(string s)
		{
			// Call to the native parsing lib PPparsing
			var tokens = NativeParsing.GetTokensRange(s);

			// get Commands with the right name
			var commands = commandsSniffer.GetValue(NativeParsing.GetMemory(s, tokens[0]));

			if (commands != null)
			{
				// there is a Command with such name
				{
					var command = commands.GetIfOneLongArgument;
					if (command != null)
						// command has one long argument
						return JustOneArgument(command, NativeParsing.GetTailMemory(s, tokens[0]).TrimStart());
				}
				tokens = tokens.Slice(1);

				var commandsWithRightArgumentCount = commands.GetValue(tokens.Length);
				if (commandsWithRightArgumentCount != null)
					// there are some Commands with the corresponing argument count
					return (commandsWithRightArgumentCount, NativeParsing.MakeMemories(s, tokens));
				else
					// there are no Commands with that argument count
					return JustOneArgument(commandsSniffer.BadArgumentCountCommand, tokens.Length);
			}
			else
				// command with that name doesn't exist
				return JustOneArgument(commandsSniffer.NotFoundCommand, s.AsMemory().TrimStart());
		}

		// Here the IEnumerable from ParseImplementation is wrapped in an Invoker
		// that invokes the elements in order
		public (IInvoker<IApplication, object[]>, object[]) Parse(string input)
		{
			var (commands, arguments) = ParseImplementation(input);
			NativeParsing.ReleaseResources();
			return (new EnumerableInvoker<IApplication, object[]>(commands), arguments);
		}
	}
}
