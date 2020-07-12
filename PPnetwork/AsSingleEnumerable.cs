using System.Collections.Generic;

namespace PPnetwork
{
    /// <summary>
    /// Static class containing extension methods used by the library.
    /// </summary>
	public static class Extensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{}"/> containing only <paramref name="x"/>.
        /// </summary>
        /// <param name="x">The object to wrap in <see cref="IEnumerable{}"/>.</param>
        /// <returns>An <see cref="IEnumerable{}"/> containing only <paramref name="x"/>.</returns>
        public static IEnumerable<T> AsSingleEnumerable<T>(this T x)
        {
            yield return x;
        }
    }
}
