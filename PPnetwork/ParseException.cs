using System;

namespace PPnetwork
{
	/// <summary>
	/// <see cref="Exception"/> for signalling an error during Command Argument parsing.
	/// </summary>
	public class ParseException : Exception
	{
		public ParseException(string message)
			: base($"parsing exception: {message}")
		{}
	}
}
