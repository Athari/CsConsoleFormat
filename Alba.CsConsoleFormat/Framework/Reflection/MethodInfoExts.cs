using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Reflection
{
    internal static class MethodInfoExts
    {
        public static bool IsVoid([NotNull] this MethodInfo @this) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).ReturnType == typeof(void);
    }
}