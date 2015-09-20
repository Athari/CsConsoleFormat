using System.Collections.Generic;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class CollectionExts
    {
        public static void AddRange<T> (this ICollection<T> @this, IEnumerable<T> items)
        {
            foreach (T item in items)
                @this.Add(item);
        }
    }
}