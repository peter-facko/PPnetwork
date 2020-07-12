using System.Collections.Generic;

namespace PPnetwork
{
	interface ICommandArgumentCountReadonlyDictionary : ISimpleReadonlyDictionary<int, IEnumerable<ICommandDescriptor>>
	{
		ISimpleCollection<ICommandDescriptor>? GetIfOneLongArgument { get; }
	}
}
