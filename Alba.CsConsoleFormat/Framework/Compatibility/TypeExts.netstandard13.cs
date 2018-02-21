using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace System
{
    internal static partial class TypeExts
    {
        [CanBeNull]
        public static ConstructorInfo GetConstructor([NotNull] this Type @this, [NotNull, ItemNotNull] Type[] types)
        {
            return @this.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c =>
                c.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }

        public static bool IsAssignableFrom([NotNull] this Type @this, [NotNull] Type type) =>
            @this.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

        public static MethodInfo GetMethod([NotNull] this Type @this, string name) =>
            @this.GetTypeInfo().GetDeclaredMethod(name);

        public static MethodInfo[] GetMethods([NotNull] this Type @this) =>
            @this.GetTypeInfo().DeclaredMethods.ToArray();
    }
}