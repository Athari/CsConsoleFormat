using System;
using System.Collections.Generic;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class EnumerableExts
    {
        public static bool AllEqual<T> (this IEnumerable<T> @this)
        {
            bool isFirst = true;
            T first = default(T);
            foreach (T item in @this) {
                if (isFirst) {
                    first = item;
                    isFirst = false;
                }
                else if (!first.EqualsValue(item))
                    return false;
            }
            return true;
        }

        public static IEnumerable<T> Concat<T> (this IEnumerable<T> @this, params T[] values)
        {
            foreach (T item in @this)
                yield return item;
            foreach (T item in values)
                yield return item;
        }

        public static IEnumerable<T> TraverseList<T> (this T root, Func<T, T> getNext) where T : class
        {
            for (T current = root; current != null; current = getNext(current))
                yield return current;
        }
    }
}