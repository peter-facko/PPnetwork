using System;

namespace PPnetwork
{
	/// <summary>
	/// <see cref="ICommandArgument"/> used to handle input when an invalid command is entered. Used only internally by the library.
	/// </summary>
	public readonly struct NotFoundCommandArgument : ICommandArgument
	{
		public readonly ReadOnlyMemory<char> Input;

		public NotFoundCommandArgument(ReadOnlyMemory<char> input)
		{
			Input = input;
		}
	}
}
