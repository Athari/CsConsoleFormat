using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Text
{
    internal static class StringExts
    {
        [ContractAnnotation("this:null => true")]
        public static bool IsNullOrEmpty([CanBeNull] this string @this) =>
            string.IsNullOrEmpty(@this);

        public static string SubstringSafe([NotNull] this string @this, int startIndex, int length) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).Substring(startIndex, Math.Min(length, @this.Length - startIndex));
    }
}