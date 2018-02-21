using System.Text;
using JetBrains.Annotations;

namespace System
{
    internal static class StringBuilderExts
    {
        public static void Clear([NotNull] this StringBuilder @this) => @this.Remove(0, @this.Length);
    }
}