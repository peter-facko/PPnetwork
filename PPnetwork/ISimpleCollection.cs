using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// An interface for a simple collection.
	/// An <see cref="IEnumerable{T}"/> that you can add elements to.
	/// </summary>
	interface ISimpleCollection<T> : IEnumerable<T>
	{
		void Add(T item);
	}
}
