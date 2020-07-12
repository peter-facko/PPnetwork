using System;

namespace PPnetwork
{
	class EndException : Exception
	{
		public string Reason { get; }
		public EndException(string reason)
		{
			Reason = reason;
		}
	}
}
