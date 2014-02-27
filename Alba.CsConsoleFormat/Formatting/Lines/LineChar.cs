using System;

namespace Alba.CsConsoleFormat
{
    [Flags]
    public enum LineChar
    {
        None = 0,
        Horizontal = 1 << 0,
        HorizontalWide = 1 << 1,
        Vertical = 1 << 2,
        VerticalWide = 1 << 3,

        MaskHorizontal = Horizontal | HorizontalWide,
        MaskVertical = Vertical | VerticalWide,
    }
}