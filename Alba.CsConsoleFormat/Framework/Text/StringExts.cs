using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Text
{
    internal static class StringExts
    {
        public static bool IsNullOrEmpty (this string @this)
        {
            return string.IsNullOrEmpty(@this);
        }

        public static string SubstringSafe (this string @this, int startIndex, int length)
        {
            return @this.Substring(startIndex, Math.Min(length, @this.Length - startIndex));
        }

        [StringFormatMethod ("format")]
        public static string Fmt (this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        [StringFormatMethod ("format")]
        public static string FmtInv (this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}