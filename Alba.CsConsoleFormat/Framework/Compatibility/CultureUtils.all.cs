using System.Globalization;
#if NET_40
using System.Threading;
#endif

namespace Alba.CsConsoleFormat.Framework.Compatibility
{
    internal static class CultureUtils
    {
        public static CultureInfo DefaultCulture
        {
            get
            {
              #if NET_40
                return Thread.CurrentThread.CurrentCulture;
              #else
                return CultureInfo.CurrentCulture;
              #endif
            }
        }

        public static CultureInfo DefaultUICulture
        {
            get
            {
              #if NET_40
                return Thread.CurrentThread.CurrentUICulture;
              #else
                return CultureInfo.CurrentUICulture;
              #endif
            }
        }
    }
}