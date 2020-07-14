using System.Collections;
using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// Simple implementation of <see cref="ISimpleCollection{T}"/>
	/// that just forwards it's methods to the underlying <see cref="ICollection{}"/>.
	/// </summary>
	/// <typeparam name="Collection">The specific <see cref="ICollection{}"/> type.</typeparam>
	/// <typeparam name="T">Element type.</typeparam>
	class SimpleCollection<Collection, T> : ISimpleCollection<T>
		where Collection : ICollection<T>, new()
	{
		readonly ICollection<T> collection;

		public SimpleCollection() => collection = new Collection();
		public void Add(T item) => collection.Add(item);
		public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
	}
}
