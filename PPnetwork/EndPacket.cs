using System;

namespace PPnetwork
{
	/// <summary>
	/// Packet used to close the connection.
	/// </summary>
	[Serializable]
	public readonly struct EndPacket : IPacket
	{
		public readonly string Reason;

		public EndPacket(string reason)
		{
			Reason = reason;
		}
	}
}
