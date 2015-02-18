using System.Collections.Generic;

namespace Alba.CsConsoleFormat.Framework.Sys
{
    internal static class ObjectExts
    {
        public static bool EqualsValue<T> (this T @this, T value)
        {
            return EqualityComparer<T>.Default.Equals(@this, value);
        }
    }
}