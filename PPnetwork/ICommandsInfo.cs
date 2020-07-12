using System;

namespace PPnetwork
{
	interface ICommandsSniffer : ISimpleReadonlyDictionary<ReadOnlyMemory<char>, ICommandArgumentCountReadonlyDictionary>
	{
		ICommandDescriptor NotFoundCommand { get; }
		ICommandDescriptor BadArgumentCountCommand { get; }
	}
}
