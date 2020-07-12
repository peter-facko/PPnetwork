using System;

namespace PPnetwork
{
	readonly struct NotFoundCommandArgument : ICommandArgument
	{
		public readonly ReadOnlyMemory<char> Input;

		public NotFoundCommandArgument(ReadOnlyMemory<char> input)
		{
			Input = input;
		}
	}
}
