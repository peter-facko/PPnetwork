using System.Collections.Generic;

namespace PPnetwork
{
	interface ISimpleCollection<T> : IEnumerable<T>
	{
		void Add(T item);
	}
}
