using System;
using System.Collections.Generic;
using System.Linq;

namespace SylphyHorn.Utils
{
	internal static class EnumerableFx
	{
		public static IEnumerable<T> Return<T>(T value)
		{
			yield return value;
		}

		public static string JoinString<T>(this IEnumerable<T> source, string separator)
			=> string.Join(separator, source is IEnumerable<string> enumerable ? enumerable : source.Select(x => x?.ToString()));
	}
}
