using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class CollectionExts
    {
        public static void AddRange<T> (this ICollection<T> @this, [InstantHandle] IEnumerable<T> items)
        {
            foreach (T item in items)
                @this.Add(item);
        }

        public static void Replace<T> (this ICollection<T> @this, [InstantHandle] IEnumerable<T> items)
        {
            @this.Clear();
            @this.AddRange(items);
        }
    }
}