using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Collections
{
    internal static class ArrayExts
    {
        public static ReadOnlyCollection<T> ToReadOnly<T>([NotNull] this T[] @this) =>
            new ReadOnlyCollection<T>(@this);
    }
}