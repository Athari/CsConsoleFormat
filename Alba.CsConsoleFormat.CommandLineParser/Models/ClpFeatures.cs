using System;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    [Flags]
    internal enum ClpFeatures
    {
        None,
        VersionUnknown = 1 << 0,
        Version19 = 1 << 1,
        Version22 = 1 << 2,
        Examples = 1 << 3,
        BuiltInHelpVersion = 1 << 4,
    }

    internal static class ClpFeaturesExts
    {
        public static bool Has(this ClpFeatures @this, ClpFeatures flag) => (@this & flag) == flag;
    }
}