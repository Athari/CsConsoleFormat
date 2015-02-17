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

        public static string RemovePrefix (this string @this, string prefix)
        {
            if (!@this.StartsWith(prefix))
                throw new ArgumentException("String '{0}' does not contain prefix '{1}'.".Fmt(@this, prefix), "prefix");
            return @this.Substring(prefix.Length);
        }

        public static string RemovePostfix (this string @this, string postfix)
        {
            if (!@this.EndsWith(postfix))
                throw new ArgumentException("String '{0}' does not contain postfix '{1}'.".Fmt(@this, postfix), "postfix");
            return @this.Remove(@this.Length - postfix.Length);
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