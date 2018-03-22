#if NET_40 || NET_STANDARD_15
using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser.Framework
{
    internal static class TypeExts
    {
      #if NET_40 || NET_STANDARD_15
        public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this Type @this, bool inherit = true)
            where T : Attribute
        {
          #if NET_FULL
            return (T[])Attribute.GetCustomAttributes(@this, inherit);
          #else
            return @this.GetTypeInfo().GetCustomAttributes<T>(inherit);
          #endif
        }
      #endif

      #if NET_40
        public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this MemberInfo @this, bool inherit = true)
            where T : Attribute
        {
            return (T[])Attribute.GetCustomAttributes(@this, inherit);
        }

        public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this Assembly @this, bool inherit = true)
            where T : Attribute
        {
            return (T[])Attribute.GetCustomAttributes(@this, inherit);
        }
      #endif
    }
}