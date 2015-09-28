using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public static class LineCharRenderer
    {
        public static readonly ILineCharRenderer Box = new BoxLineCharRenderer();
        public static readonly ILineCharRenderer None = new CharLineCharRenderer('\0');
        public static readonly ILineCharRenderer Simple = new SimpleLineCharRenderer();
        public static ILineCharRenderer Char (char c) => new CharLineCharRenderer(c);

        private abstract class LineCharRendererBase : ILineCharRenderer
        {
            public abstract char GetChar (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom);

            protected static Exception GetCharException (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom)
            {
                return new NotSupportedException($"Line joint not supported: {charCenter} ({charLeft} {charTop} {charRight} {charBottom}).");
            }
        }

        private class BoxLineCharRenderer : LineCharRendererBase
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

            [SuppressMessage ("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Logic is straightforward, complexity comes from inability to iterate over variables.")]
            public override char GetChar (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom)
            {
                if (charCenter.IsEmpty())
                    return '\0';
                // 0 connections
                if (charLeft.IsNone() && charTop.IsNone() && charRight.IsNone() && charBottom.IsNone())
                    return GetSimpleChar(charCenter, MapSimple);

                bool connectLeft = charCenter.IsHorizontal() && charLeft.IsHorizontal();
                bool connectTop = charCenter.IsVertical() && charTop.IsVertical();
                bool connectRight = charCenter.IsHorizontal() && charRight.IsHorizontal();
                bool connectBottom = charCenter.IsVertical() && charBottom.IsVertical();
                // 1 connection
                if ((connectLeft ? 1 : 0) + (connectTop ? 1 : 0) + (connectRight ? 1 : 0) + (connectBottom ? 1 : 0) <= 1)
                    return GetSimpleChar(charCenter, MapSimple);
                // 4 connections
                if (connectLeft && connectTop && connectRight && connectBottom)
                    return GetChar(charCenter, MapLeftTopRightBottom);
                // 3 connections
                if (connectLeft && connectTop && connectRight)
                    return GetChar(charCenter, MapLeftTopRight);
                if (connectLeft && connectTop && connectBottom)
                    return GetChar(charCenter, MapLeftTopBottom);
                if (connectLeft && connectRight && connectBottom)
                    return GetChar(charCenter, MapLeftRightBottom);
                if (connectTop && connectRight && connectBottom)
                    return GetChar(charCenter, MapTopRightBottom);
                // 2 connections
                if (connectTop && connectLeft)
                    return GetChar(charCenter, MapTopLeft);
                if (connectTop && connectRight)
                    return GetChar(charCenter, MapTopRight);
                if (connectBottom && connectRight)
                    return GetChar(charCenter, MapBottomRight);
                if (connectBottom && connectLeft)
                    return GetChar(charCenter, MapBottomLeft);
                if ((connectLeft && connectRight) || (connectTop && connectBottom))
                    return GetSimpleChar(charCenter, MapSimple);
                throw GetCharException(charCenter, charLeft, charTop, charRight, charBottom);
            }

            private static char GetSimpleChar (LineChar charCenter, char[] map)
            {
                Debug.Assert(map.Length == 8);
                switch (charCenter) {
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
                        throw new ArgumentOutOfRangeException(nameof(charCenter));
                }
            }

            private static char GetChar (LineChar charCenter, char[] map)
            {
                Debug.Assert(map.Length == 4);
                if (!charCenter.IsHorizontalWide() && !charCenter.IsVerticalWide())
                    return map[0];
                if (charCenter.IsHorizontalWide() && !charCenter.IsVerticalWide())
                    return map[1];
                if (!charCenter.IsHorizontalWide() && charCenter.IsVerticalWide())
                    return map[2];
                if (charCenter.IsHorizontalWide() && charCenter.IsVerticalWide())
                    return map[3];
                throw new ArgumentOutOfRangeException(nameof(charCenter));
            }
        }

        private class SimpleLineCharRenderer : LineCharRendererBase
        {
            public override char GetChar (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom)
            {
                if (charCenter.IsEmpty())
                    return '\0';
                if (charCenter.IsHorizontal() && charCenter.IsVertical())
                    return '+';
                if (charCenter.IsHorizontal())
                    return charCenter.IsHorizontalWide() ? '=' : '-';
                if (charCenter.IsVertical())
                    return '|';
                throw GetCharException(charCenter, charLeft, charTop, charRight, charBottom);
            }
        }

        private class CharLineCharRenderer : LineCharRendererBase
        {
            private readonly char _c;

            public CharLineCharRenderer (char c)
            {
                _c = c;
            }

            public override char GetChar (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom)
            {
                return _c;
            }
        }
    }
}