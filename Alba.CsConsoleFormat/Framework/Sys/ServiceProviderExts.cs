using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Sys
{
    internal static class ServiceProviderExts
    {
        public static T GetService<T>([NotNull] this IServiceProvider @this) =>
            (T)(@this ?? throw new ArgumentNullException(nameof(@this))).GetService(typeof(T));
    }
}