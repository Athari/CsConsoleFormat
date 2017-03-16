namespace Alba.CsConsoleFormat
{
    internal static class ValueProviders
    {
        private const string _AlbaCsConsoleFormat = nameof(Alba) + "." + nameof(CsConsoleFormat) + ".";
        public const string Chars = _AlbaCsConsoleFormat + nameof(Chars);
        public const string ColorMaps = _AlbaCsConsoleFormat + nameof(ColorMaps);
    }
}