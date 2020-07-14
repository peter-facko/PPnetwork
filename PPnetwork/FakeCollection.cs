using System;
using System.Collections;
using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// An <see cref="IEnumerable{}"/> that acts as a <see cref="ISimpleCollection{T}"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class FakeCollection<T> : ISimpleCollection<T>
	{
		readonly IEnumerable<T> Enumerable;

		public FakeCollection(IEnumerable<T> enumerable)
			=> Enumerable = enumerable;
		/// <summary>
		/// Not supported. Is an <see cref="IEnumerable{}"/> in diguise.
		/// </summary>
		/// <param name="_"></param>
		public void Add(T _)
			=> throw new Exception();
		public IEnumerator<T> GetEnumerator()
			=> Enumerable.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator()
			=> Enumerable.GetEnumerator();
	}
}
