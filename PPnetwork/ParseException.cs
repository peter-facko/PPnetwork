using System;

namespace PPnetwork
{
	public class ParseException : Exception
	{
		public ParseException(string message)
			: base($"parsing exception: {message}")
		{}
	}
}
