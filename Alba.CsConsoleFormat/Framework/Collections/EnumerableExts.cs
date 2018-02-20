using System;
using System.Collections.Generic;
using Alba.CsConsoleFormat.Framework.Sys;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class EnumerableExts
    {
        public static bool AllEqual<T>([NotNull, InstantHandle] this IEnumerable<T> @this)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            bool isFirst = true;
            T first = default;
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

        public static IEnumerable<T> Concat<T>([NotNull] this IEnumerable<T> @this, [NotNull] params T[] values)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            foreach (T item in @this)
                yield return item;
            foreach (T item in values)
                yield return item;
        }

        public static IEnumerable<T> TraverseList<T>([CanBeNull] this T root, [NotNull] Func<T, T> getNext) where T : class
        {
            if (getNext == null)
                throw new ArgumentNullException(nameof(getNext));
            for (T current = root; current != null; current = getNext(current))
                yield return current;
        }
    }
}