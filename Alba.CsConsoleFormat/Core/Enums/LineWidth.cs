namespace Alba.CsConsoleFormat
{
    public enum LineWidth
    {
        None = 0,
        Single = 1,
        Wide = 2,
    }

    internal static class LineWidthExts
    {
        public static int ToCharWidth (this LineWidth @this)
        {
            return @this == LineWidth.None ? 0 : 1;
        }
    }
}