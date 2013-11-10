using System;
using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Service
{
	public static class Extensions
	{
		public static IEnumerable<IEnumerable<T>> PartitionBy<T>(this IEnumerable<T> source, int partitionSize)
		{
			return source
				.Select((o, i) => new
				{
					Index = i,
					Item = o,
				})
				.GroupBy(o => o.Index / partitionSize, o => o.Item);
		}

		readonly static System.Security.Cryptography.MD5 Md5 = System.Security.Cryptography.MD5.Create();

		public static string ComputeMd5(this string that)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(that);
			var hash = Md5.ComputeHash(bytes);
			var hashString = hash.Select(b => b.ToString("x2"));
			var result = String.Join("-", hashString);
			return result;
		}
	}
}