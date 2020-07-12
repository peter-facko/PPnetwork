using System;

namespace PPnetwork
{
	abstract class ParseException : Exception
	{
		public ParseException(string message)
			: base($"parsing exception: {message}")
		{}
	}
}
