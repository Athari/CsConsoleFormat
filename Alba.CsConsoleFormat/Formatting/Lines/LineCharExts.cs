namespace Alba.CsConsoleFormat
{
    internal static class LineCharExts
    {
        public static bool IsEmpty (this LineChar @this)
        {
            return @this.IsNone() || !@this.IsHorizontal() && !@this.IsVertical();
        }

        public static bool IsNone (this LineChar @this)
        {
            return @this == LineChar.None;
        }

        public static bool IsHorizontal (this LineChar @this)
        {
            return (@this & LineChar.Horizontal) != 0;
        }

        public static bool IsHorizontalWide (this LineChar @this)
        {
            return (@this & LineChar.HorizontalWide) != 0;
        }

        public static bool IsVertical (this LineChar @this)
        {
            return (@this & LineChar.Vertical) != 0;
        }

        public static bool IsVerticalWide (this LineChar @this)
        {
            return (@this & LineChar.VerticalWide) != 0;
        }
    }
}