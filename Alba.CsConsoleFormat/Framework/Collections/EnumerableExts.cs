using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class EnumerableExts
    {
        public static IEnumerable<T> TraverseList<T> (this T root, Func<T, T> getNext) where T : class
        {
            for (T current = root; current != null; current = getNext(current))
                yield return current;
        }
    }
}