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
        OptionsValues = 1 << 7,
        OptionsDefaultValue = 1 << 8,
        SubOptions = 1 << 9,
        BuiltInOptions = 1 << 10,
        HiddenOptions = 1 << 11,
        Examples = 1 << 12,

        AssemblyAllMeta = AssemblyTitle | AssemblyVersion | AssemblyCopyright | AssemblyLicense,

        DefaultVersion = AssemblyAllMeta,
        DefaultErrors = AssemblyAllMeta | AssemblyUsage | Errors | Options | BuiltInOptions,
        DefaultOptions = AssemblyAllMeta | AssemblyUsage | Errors | Examples | Options | OptionsValues | OptionsDefaultValue | BuiltInOptions,

        All = AssemblyAllMeta | AssemblyUsage | DefaultOptions | SubOptions,
    }

    internal static class HelpPartsExts
    {
        public static bool Has(this HelpParts @this, HelpParts flag) => (@this & flag) == flag;
    }
}