using System;

namespace Alba.CsConsoleFormat.Framework.Text
{
    internal static class StringExts
    {
        public static bool IsNullOrEmpty (this string @this) => string.IsNullOrEmpty(@this);

        public static string SubstringSafe (this string @this, int startIndex, int length) =>
            @this.Substring(startIndex, Math.Min(length, @this.Length - startIndex));
    }
}