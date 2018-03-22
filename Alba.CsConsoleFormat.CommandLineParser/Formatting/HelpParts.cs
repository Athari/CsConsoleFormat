using System;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    [Flags]
    public enum HelpParts
    {
        None,
        AssemblyTitle = 1 << 0,
        AssemblyVersion = 1 << 1,
        AssemblyCopyright = 1 << 2,
        AssemblyLicense = 1 << 3,
        AssemblyUsage = 1 << 4,
        Errors = 1 << 5,
        Options = 1 << 6,
        SubOptions = 1 << 7,
        Examples = 1 << 8,

        AssemblyMeta = AssemblyTitle | AssemblyVersion | AssemblyCopyright | AssemblyLicense | AssemblyUsage,
        DefaultVersion = AssemblyMeta & ~AssemblyUsage,
        DefaultErrors = AssemblyMeta | Errors,
        DefaultOptions = AssemblyMeta | Errors | Examples | Options,
        All = AssemblyMeta | Errors | Examples | Options | SubOptions,
    }

    internal static class HelpPartsExts
    {
        public static bool Has(this HelpParts @this, HelpParts flag) => (@this & flag) == flag;
    }
}