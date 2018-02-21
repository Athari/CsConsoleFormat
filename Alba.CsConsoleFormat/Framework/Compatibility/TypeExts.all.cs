using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.Annotations;

namespace System
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Conditional compilation.")]
    internal static partial class TypeExts
    {
        public static Assembly GetAssembly([NotNull] this Type @this)
        {
          #if NET_FULL
            return @this.Assembly;
          #else
            return @this.GetTypeInfo().Assembly;
          #endif
        }
    }
}