using System.Reflection;

namespace Alba.CsConsoleFormat.Framework.Reflection
{
    internal static class MethodInfoExts
    {
        public static bool IsVoid (this MethodInfo @this)
        {
            return @this.ReturnType == typeof(void);
        }
    }
}