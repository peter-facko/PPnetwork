using System;

namespace PPnetwork
{
	/// <summary>
	/// Sniffer interface used by the Parser,
	/// which needs to just read Descriptors from the Sniffer, not write them to it.
	/// </summary>
	interface ICommandsSniffer : ISimpleReadonlyDictionary<ReadOnlyMemory<char>, ICommandArgumentCountReadonlyDictionary>
	{
		ICommandDescriptor NotFoundCommand { get; }
		ICommandDescriptor BadArgumentCountCommand { get; }
	}
}
