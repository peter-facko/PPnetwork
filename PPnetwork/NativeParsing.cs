using System;
using System.Text;
using System.Runtime.InteropServices;

namespace PPnetwork
{
	public static class NativeParsing
	{
		/// <summary>
		/// Sets the input and output encoding of the console to ASCII.
		/// </summary>
		public static void SetEncoding()
		{
			Console.InputEncoding = Encoding.ASCII;
			Console.OutputEncoding = Console.InputEncoding;
		}


		/// <summary>
		/// Represents a range of contiguous elements in memory.
		/// </summary>
		/// <typeparam name="T">The type of elements in the range.</typeparam>
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public unsafe readonly struct Range<T>
			where T : unmanaged
		{
			/// <summary>
			/// Pointer to the first element of the Range.
			/// </summary>
			public readonly T* Begin;
			/// <summary>
			/// Pointer to memory after the last element. End - 1 points to the last element.
			/// </summary>
			public readonly T* End;

			/// <summary>
			/// Creates a Range with specified begin and end pointers.
			/// </summary>
			/// <param name="begin">Pointer to the first element fo the range.</param>
			/// <param name="end">Pointer to memory after the last element. end - 1 should point to the last element.</param>
			public Range(T* begin, T* end)
			{
				Begin = begin;
				End = end;
			}

			/// <summary>
			/// Gets the number of elements in the Range.
			/// </summary>
			public int Length => (int)(End - Begin);

			/// <summary>
			/// Creates a Span representing the same range as the Range.
			/// </summary>
			public Span<T> AsSpan() => new Span<T>(Begin, Length);

			public static implicit operator Span<T>(Range<T> s) => s.AsSpan();
			public static implicit operator ReadOnlySpan<T>(Range<T> s) => (Span<T>)s;
		}



		[DllImport("PPparsing.dll")]
		static extern Range<Range<char>> GetTokensRangeImplementation(Range<char> input);

		/// <summary>
		/// Creates a Span of tokens in the <paramref name="input"/>.
		/// Modifies <paramref name="input"/>.
		/// See the documentation for the definition of a token.
		/// </summary>
		/// <param name="input">The string to process.</param>
		/// <returns>Tokens in the <paramref name="input"/>. Token is represented as a range of chars.</returns>
		public static unsafe Span<Range<char>> GetTokensRange(string input)
		{
			Span<Range<char>> s;
			fixed (char* ptr = input)
			{
				s = GetTokensRangeImplementation(new Range<char>(ptr, ptr + input.Length));
			}
			return s;
		}

		/// <summary>
		/// Gets the index of the first element of <paramref name="span"/> in <paramref name="s"/>. Undefined behavior if <paramref name="span"/> is not a range from <paramref name="s"/>.
		/// </summary>
		/// <param name="s">The string inside which <paramref name="span"/> points</param>
		/// <param name="span">A subrange of <paramref name="s"/></param>
		/// <returns>Index of the first element of <paramref name="span"/> in <paramref name="s"/>.</returns>
		unsafe static long GetOffsetFromStart(string s, ReadOnlySpan<char> span)
		{
			fixed (char* string_ptr = s, span_ptr = span)
			{
				return span_ptr - string_ptr;
			}
		}

		/// <summary>
		/// Gets a <see cref="ReadOnlyMemory{T}"/> to the part of <paramref name="s"/> that <paramref name="token"/> represents. Undefined behavior if <paramref name="token"/> is not a subrange of <paramref name="s"/>.
		/// </summary>
		/// <param name="s">The string inside which <paramref name="token"/> points</param>
		/// <param name="token">A subrange of <paramref name="s"/></param>
		/// <returns><see cref="ReadOnlyMemory{}"/> to the part of <paramref name="s"/> that <paramref name="token"/> represents.</returns>
		public static ReadOnlyMemory<char> GetMemory(string s, ReadOnlySpan<char> token)
			=> s.AsMemory().Slice((int)GetOffsetFromStart(s, token), token.Length);

		/// <summary>
		/// Gets a <see cref="ReadOnlyMemory{T}"/> to the part of <paramref name="s"/> after <paramref name="token"/>. Undefined behavior if <paramref name="token"/> is not a subrange of <paramref name="s"/>.
		/// </summary>
		/// <param name="s">The string inside which <paramref name="token"/> points</param>
		/// <param name="token">A subrange of <paramref name="s"/></param>
		/// <returns><see cref="ReadOnlyMemory{}"/> to the part of <paramref name="s"/> after <paramref name="token"/>.</returns>
		public static ReadOnlyMemory<char> GetTailMemory(string s, ReadOnlySpan<char> token)
			=> s.AsMemory().Slice(token.Length);

		/// <summary>
		/// Creates an <see cref="object"/>[] of <see cref="ReadOnlyMemory{T}"/> from <paramref name="tokens"/>, which are subranges of <paramref name="s"/>.
		/// </summary>
		/// <param name="s">The string inside which all from <paramref name="tokens"/> point</param>
		/// <param name="tokens">Subranges of <paramref name="s"/></param>
		/// <returns><see cref="object"/>[] of <see cref="ReadOnlyMemory{T}"/>.</returns>
		public static object[] MakeMemories(string s, Span<Range<char>> tokens)
		{
			var arr = new object[tokens.Length];

			for (int i = 0; i != arr.Length; ++i)
				arr[i] = GetMemory(s, tokens[i]);

			return arr;
		}


		/// <summary>
		/// Releases resources allocated by the native parsing during a call to <see cref="GetTokensRange"/>.
		/// </summary>
		[DllImport("PPparsing.dll")]
		public static extern void ReleaseResources();
	}
}
