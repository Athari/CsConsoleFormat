using System;

namespace Alba.CsConsoleFormat.Framework.Sys
{
    internal static class ServiceProviderExts
    {
        public static T GetService<T> (this IServiceProvider @this)
        {
            return (T)@this.GetService(typeof(T));
        }
    }
}