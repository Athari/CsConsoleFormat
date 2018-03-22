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

        public static Type GetBaseType([NotNull] this Type @this)
        {
          #if NET_FULL
            return @this.BaseType;
          #else
            return @this.GetTypeInfo().BaseType;
          #endif
        }

        public static bool Is([NotNull] this Type @this, Type type) => type.IsAssignableFrom(@this);

        public static bool Is<T>([NotNull] this Type @this) => @this.Is(typeof(T));
    }
}