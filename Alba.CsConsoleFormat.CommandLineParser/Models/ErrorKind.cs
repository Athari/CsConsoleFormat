namespace Alba.CsConsoleFormat.CommandLineParser
{
    public enum ErrorKind
    {
        ParseError,
        Error,
        Info,
        Warning,
        HelpVerb,
        VersionVerb,
    }

    internal static class ErrorKindExts
    {
        public static bool IsNormal(this ErrorKind @this) =>
            @this == ErrorKind.Error || @this == ErrorKind.ParseError || @this == ErrorKind.Warning || @this == ErrorKind.Info;
    }
}