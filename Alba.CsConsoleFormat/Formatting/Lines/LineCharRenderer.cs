using System;
using System.Diagnostics;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    public static class LineCharRenderer
    {
        public static readonly ILineCharRenderer Box = new BoxLineCharRenderer();
        public static readonly ILineCharRenderer None = new CharLineCharRenderer('\0');
        public static readonly ILineCharRenderer Simple = new SimpleLineCharRenderer();

        public static ILineCharRenderer Char (char chr)
        {
            return new CharLineCharRenderer(chr);
        }

        private class BoxLineCharRenderer : ILineCharRenderer
        {
            private static readonly char[] MapSimple = { '─', '═', '│', '║', '┼', '╪', '╫', '╬' };
            private static readonly char[] MapLeftTopRightBottom = { '┼', '╪', '╫', '╬' };
            private static readonly char[] MapLeftTopRight = { '┴', '╧', '╨', '╩' };
            private static readonly char[] MapLeftTopBottom = { '┤', '╡', '╢', '╣' };
            private static readonly char[] MapLeftRightBottom = { '┬', '╤', '╥', '╦' };
            private static readonly char[] MapTopRightBottom = { '├', '╞', '╟', '╠' };
            private static readonly char[] MapTopLeft = { '┘', '╛', '╜', '╝' };
            private static readonly char[] MapTopRight = { '└', '╘', '╙', '╚' };
            private static readonly char[] MapBottomLeft = { '┐', '╕', '╖', '╗' };
            private static readonly char[] MapBottomRight = { '┌', '╒', '╓', '╔' };

            public char GetChar (LineChar chr, LineChar chrLeft, LineChar chrTop, LineChar chrRight, LineChar chrBottom)
            {
                if (chr.IsEmpty())
                    return '\0';
                // 0 connections
                if (chrLeft.IsNone() && chrTop.IsNone() && chrRight.IsNone() && chrBottom.IsNone())
                    return GetSimpleChar(chr, MapSimple);

                bool connectLeft = chr.IsHorizontal() && chrLeft.IsHorizontal();
                bool connectTop = chr.IsVertical() && chrTop.IsVertical();
                bool connectRight = chr.IsHorizontal() && chrRight.IsHorizontal();
                bool connectBottom = chr.IsVertical() && chrBottom.IsVertical();
                // 1 connection
                if ((connectLeft ? 1 : 0) + (connectTop ? 1 : 0) + (connectRight ? 1 : 0) + (connectBottom ? 1 : 0) <= 1)
                    return GetSimpleChar(chr, MapSimple);
                // 4 connections
                if (connectLeft && connectTop && connectRight && connectBottom)
                    return GetChar(chr, MapLeftTopRightBottom);
                // 3 connections
                if (connectLeft && connectTop && connectRight)
                    return GetChar(chr, MapLeftTopRight);
                if (connectLeft && connectTop && connectBottom)
                    return GetChar(chr, MapLeftTopBottom);
                if (connectLeft && connectRight && connectBottom)
                    return GetChar(chr, MapLeftRightBottom);
                if (connectTop && connectRight && connectBottom)
                    return GetChar(chr, MapTopRightBottom);
                // 2 connections
                if (connectTop && connectLeft)
                    return GetChar(chr, MapTopLeft);
                if (connectTop && connectRight)
                    return GetChar(chr, MapTopRight);
                if (connectBottom && connectRight)
                    return GetChar(chr, MapBottomRight);
                if (connectBottom && connectLeft)
                    return GetChar(chr, MapBottomLeft);
                if ((connectLeft && connectRight) || (connectTop && connectBottom))
                    return GetSimpleChar(chr, MapSimple);
                throw new NotSupportedException("Line joint not supported: {0} ({1} {2} {3} {4})."
                    .Fmt(chr, chrLeft, chrTop, chrRight, chrBottom));
            }

            private static char GetSimpleChar (LineChar chr, char[] map)
            {
                Debug.Assert(map.Length == 8);
                switch (chr) {
                    case LineChar.Horizontal:
                        return map[0];
                    case LineChar.Horizontal | LineChar.HorizontalWide:
                        return map[1];
                    case LineChar.Vertical:
                        return map[2];
                    case LineChar.Vertical | LineChar.VerticalWide:
                        return map[3];
                    case LineChar.Horizontal | LineChar.Vertical:
                        return map[4];
                    case LineChar.Horizontal | LineChar.Vertical | LineChar.HorizontalWide:
                        return map[5];
                    case LineChar.Horizontal | LineChar.Vertical | LineChar.VerticalWide:
                        return map[6];
                    case LineChar.Horizontal | LineChar.Vertical | LineChar.HorizontalWide | LineChar.VerticalWide:
                        return map[7];
                    default:
                        throw new ArgumentOutOfRangeException("chr");
                }
            }

            private static char GetChar (LineChar chr, char[] map)
            {
                Debug.Assert(map.Length == 4);
                if (!chr.IsHorizontalWide() && !chr.IsVerticalWide())
                    return map[0];
                if (chr.IsHorizontalWide() && !chr.IsVerticalWide())
                    return map[1];
                if (!chr.IsHorizontalWide() && chr.IsVerticalWide())
                    return map[2];
                if (chr.IsHorizontalWide() && chr.IsVerticalWide())
                    return map[3];
                throw new ArgumentOutOfRangeException("chr");
            }
        }

        private class SimpleLineCharRenderer : ILineCharRenderer
        {
            public char GetChar (LineChar chr, LineChar chrLeft, LineChar chrTop, LineChar chrRight, LineChar chrBottom)
            {
                throw new NotImplementedException();
            }
        }

        private class CharLineCharRenderer : ILineCharRenderer
        {
            private readonly char _chr;

            public CharLineCharRenderer (char chr)
            {
                _chr = chr;
            }

            public char GetChar (LineChar chr, LineChar chrLeft, LineChar chrTop, LineChar chrRight, LineChar chrBottom)
            {
                return _chr;
            }
        }
    }
}