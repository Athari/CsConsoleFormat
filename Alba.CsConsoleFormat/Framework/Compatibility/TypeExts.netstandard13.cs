using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace System
{
    internal static partial class TypeExts
    {
        public static bool IsAssignableFrom([NotNull] this Type @this, [NotNull] Type type) =>
            @this.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
    }
}