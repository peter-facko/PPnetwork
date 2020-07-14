using System;

namespace PPnetwork
{
	/// <summary>
	/// <see cref="Exception"/> used to exit Packet receiving loop in Connections.
	/// </summary>
	class EndException : Exception
	{
		public string Reason { get; }
		public EndException(string reason)
		{
			Reason = reason;
		}
	}
}
