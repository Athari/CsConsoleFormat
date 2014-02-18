using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class ListExts
    {
        public static void RemoveAll<T> (this IList<T> @this, Predicate<T> match)
        {
            for (int i = @this.Count - 1; i >= 0; i--)
                if (match(@this[i]))
                    @this.RemoveAt(i);
        }
    }
}