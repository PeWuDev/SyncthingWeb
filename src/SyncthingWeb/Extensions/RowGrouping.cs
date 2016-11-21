using System;
using System.Collections.Generic;

namespace SyncthingWeb.Extensions
{
    public static class RowGrouping
    {
        public static IEnumerable<IEnumerable<T>> DivideIntoRows<T>(this IEnumerable<T> source, int perRow)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (perRow < 1) throw new ArgumentOutOfRangeException(nameof(perRow), "Must be greater than 1");

            var enumerator = source.GetEnumerator();

            List<T> bucket = new List<T>();

            while (enumerator.MoveNext())
            {
                bucket.Add(enumerator.Current);
                if (bucket.Count == perRow)
                {
                    yield return new List<T>(bucket);
                    bucket.Clear();
                }
            }

            if (bucket.Count > 0) yield return new List<T>(bucket);
        }
    }
}