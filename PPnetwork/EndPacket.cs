using System;

namespace PPnetwork
{
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
