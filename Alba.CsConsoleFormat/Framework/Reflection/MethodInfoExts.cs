using System.Reflection;

namespace Alba.CsConsoleFormat.Framework.Reflection
{
    internal static class MethodInfoExts
    {
        public static bool IsVoid(this MethodInfo @this) => @this.ReturnType == typeof(void);
    }
}