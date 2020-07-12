using System.Collections.Generic;

namespace PPnetwork
{
	public static class Extensions
    {
        public static IEnumerable<T> AsSingleEnumerable<T>(this T x)
        {
            yield return x;
        }
    }
}
