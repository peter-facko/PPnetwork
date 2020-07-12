using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PPnetwork
{
	/// <summary>
	/// Compares two <see cref="ReadOnlyMemory{T}"/> of <see cref="char"/> lexicographically.
	/// </summary>
	public class MemoryStringComparer : IComparer<ReadOnlyMemory<char>>
	{
		public int Compare([AllowNull] ReadOnlyMemory<char> x, [AllowNull] ReadOnlyMemory<char> y)
			=> x.Span.SequenceCompareTo(y.Span);
	}
}
