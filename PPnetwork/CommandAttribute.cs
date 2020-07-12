using System;

namespace PPnetwork
{
	/// <summary>
	/// Flag enum representing optimizations to do during command sniffing and parsing.
	/// </summary>
	[Flags]
	public enum CommandFlags
	{
		/// <summary>
		/// No optimization.
		/// </summary>
		None = 0b000,
		/// <summary>
		/// The name of command is unique.
		/// </summary>
		UniqueName = 0b001,
		/// <summary>
		/// The argument count of this command is unique among commands with the same name.
		/// </summary>
		UniqueArgumentCount = 0b010,
		/// <summary>
		/// Command accepts one long argument, so parsing shouldn't divide the input into tokens.
		/// </summary>
		OneLongArgument = 0b100,
	}

	/// <summary>
	/// <see cref="Attribute"/> providing info about a command. Should be applied to types implementing <see cref="ICommandArgument"/>.
	/// </summary>
	public class CommandAttribute : Attribute
	{
		/// <summary>
		/// Name of the command.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Priority of the command. Considered only when commands have the same name and argument count. Then they are tried in descenging order by priority.
		/// </summary>
		public int Priority { get; }
		/// <summary>
		/// Optimization flags.
		/// </summary>
		readonly CommandFlags Flags;

		/// <summary>
		/// Don't use if <paramref name="flags"/> is <see cref="CommandFlags.None"/>. These commands should have different priorities, so use <see cref="CommandAttribute(string, int)"/>.
		/// </summary>
		public CommandAttribute(string name, CommandFlags flags)
		  : this(name, 0, flags)
		{ }
		/// <summary>
		/// Use when there are no valid optimizations for the command.
		/// </summary>
		public CommandAttribute(string name, int priority)
		  : this(name, priority, CommandFlags.None)
		{ }
		CommandAttribute(string name, int priority, CommandFlags flags)
		{
			Name = name;
			Priority = priority;
			Flags = flags;
		}

		public bool HasOneLongArgument => Flags.HasFlag(CommandFlags.OneLongArgument);
		public bool HasUniqueName => Flags.HasFlag(CommandFlags.UniqueName);
		public bool HasUniqueArgumentCount => HasOneLongArgument || HasUniqueName || Flags.HasFlag(CommandFlags.UniqueArgumentCount);
	}
}
