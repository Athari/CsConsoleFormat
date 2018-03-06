using System;
using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public static class LineCharRenderer
    {
        public static readonly ILineCharRenderer Box = new BoxLineCharRenderer(false);
        public static readonly ILineCharRenderer BoxExtended = new BoxLineCharRenderer(true);
        public static readonly ILineCharRenderer None = new CharLineCharRenderer('\0');
        public static readonly ILineCharRenderer Simple = new SimpleLineCharRenderer();
        public static ILineCharRenderer Char(char c) => new CharLineCharRenderer(c);

        private abstract class LineCharRendererBase : ILineCharRenderer
        {
            public abstract char GetChar(LineChar lineChar);

            protected static Exception GetCharException(LineChar lineChar)
            {
                return new NotSupportedException($"Line char '{lineChar}' supported.");
            }
        }

        private sealed class BoxLineCharRenderer : LineCharRendererBase
        {
            private static readonly char[] CharsDoubleHorizontal = { '─', '═' };
            private static readonly char[] CharsDoubleVertical = { '│', '║' };
            private static readonly char[] CharsDoubleCornerTopLeft = { '┘', '╛', '╜', '╝' };
            private static readonly char[] CharsDoubleCornerTopRight = { '└', '╘', '╙', '╚' };
            private static readonly char[] CharsDoubleCornerBottomLeft = { '┐', '╕', '╖', '╗' };
            private static readonly char[] CharsDoubleCornerBottomRight = { '┌', '╒', '╓', '╔' };
            private static readonly char[] CharsDoubleTLeft = { '┤', '╡', '╢', '╣' };
            private static readonly char[] CharsDoubleTTop = { '┴', '╧', '╨', '╩' };
            private static readonly char[] CharsDoubleTRight = { '├', '╞', '╟', '╠' };
            private static readonly char[] CharsDoubleTBottom = { '┬', '╤', '╥', '╦' };
            private static readonly char[] CharsDoubleCross = { '┼', '╪', '╫', '╬' };

            private static readonly char[] CharsHeavyLeft = { '╴', '╸' };
            private static readonly char[] CharsHeavyTop = { '╵', '╹' };
            private static readonly char[] CharsHeavyRight = { '╶', '╺' };
            private static readonly char[] CharsHeavyBottom = { '╷', '╻' };
            private static readonly char[] CharsHeavyHorizontal = { '─', '━', '╼', '╾' };
            private static readonly char[] CharsHeavyVertical = { '│', '┃', '╽', '╿' };
            private static readonly char[] CharsHeavyCornerTopLeft = { '┘', '┙', '┚', '┛' };
            private static readonly char[] CharsHeavyCornerTopRight = { '└', '┕', '┖', '┗' };
            private static readonly char[] CharsHeavyCornerBottomLeft = { '┐', '┑', '┒', '┓' };
            private static readonly char[] CharsHeavyCornerBottomRight = { '┌', '┍', '┎', '┏' };
            private static readonly char[] CharsHeavyTLeft = { '┤', '┥', '┨', '┫' };
            private static readonly char[] CharsHeavyTLeft1 = { '┦', '┧' };
            private static readonly char[] CharsHeavyTLeft2 = { '┩', '┪' };
            private static readonly char[] CharsHeavyTTop = { '┴', '┷', '┸', '┻' };
            private static readonly char[] CharsHeavyTTop1 = { '┵', '┶' };
            private static readonly char[] CharsHeavyTTop2 = { '┹', '┺' };
            private static readonly char[] CharsHeavyTRight = { '├', '┝', '┠', '┣' };
            private static readonly char[] CharsHeavyTRight1 = { '┞', '┟' };
            private static readonly char[] CharsHeavyTRight2 = { '┡', '┢' };
            private static readonly char[] CharsHeavyTBottom = { '┬', '┯', '┰', '┳' };
            private static readonly char[] CharsHeavyTBottom1 = { '┭', '┮' };
            private static readonly char[] CharsHeavyTBottom2 = { '┱', '┲' };
            private static readonly char[] CharsHeavyCross = { '┼', '┿', '╂', '╋' };
            private static readonly char[] CharsHeavyCross1 = { '┽', '╀', '┾', '╁' };
            private static readonly char[] CharsHeavyCross2Line = { '┿', '╂' };
            private static readonly char[] CharsHeavyCross2Corner = { '╃', '╄', '╅', '╆' };
            private static readonly char[] CharsHeavyCross3 = { '╉', '╇', '╊', '╈' };

            private readonly bool _supportExtended;

            public BoxLineCharRenderer(bool supportExtended)
            {
                _supportExtended = supportExtended;
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Logic is straightforward, complexity comes from inability to iterate over variables.")]
            public override char GetChar(LineChar lineChar)
            {
                LineWidth left = lineChar.Left;
                LineWidth top = lineChar.Top;
                LineWidth right = lineChar.Right;
                LineWidth bottom = lineChar.Bottom;

                bool hasSingle = HasLineWidth(LineWidth.Single);
                bool hasHeavy = HasLineWidth(LineWidth.Heavy);
                bool hasDouble = HasLineWidth(LineWidth.Double);
                int lineCount = CountLines(width => width != LineWidth.None);

                if (hasHeavy && hasDouble)
                    throw new InvalidOperationException("Heavy and Double line widths can't be combined.");
                if (hasHeavy && !_supportExtended)
                    throw new NotSupportedException("Heavy line width not supported by Box line char renderer. Use BoxExtended instead.");

                if (!hasSingle && !hasHeavy && !hasDouble)
                    return '\0';
                else if (hasHeavy) {
                    throw new NotImplementedException("Heavy line width not supported yet.");
                }
                else {
                    int horizontalIndex = LineWidthExts.Max(left, right) == LineWidth.Single ? 0 : 1;
                    int verticalIndex = LineWidthExts.Max(top, bottom) == LineWidth.Single ? 0 : 1;
                    int combinedIndex = verticalIndex * 2 + horizontalIndex;
                    switch (lineCount) {
                        case 1:
                            return left != LineWidth.None || right != LineWidth.None
                                ? CharsDoubleHorizontal[horizontalIndex]
                                : CharsDoubleVertical[verticalIndex];
                        case 2:
                            if (left != LineWidth.None && right != LineWidth.None)
                                return CharsDoubleHorizontal[horizontalIndex];
                            else if (top != LineWidth.None && bottom != LineWidth.None)
                                return CharsDoubleVertical[verticalIndex];
                            else if (top != LineWidth.None && left != LineWidth.None)
                                return CharsDoubleCornerTopLeft[combinedIndex];
                            else if (top != LineWidth.None && right != LineWidth.None)
                                return CharsDoubleCornerTopRight[combinedIndex];
                            else if (bottom != LineWidth.None && left != LineWidth.None)
                                return CharsDoubleCornerBottomLeft[combinedIndex];
                            else if (bottom != LineWidth.None && right != LineWidth.None)
                                return CharsDoubleCornerBottomRight[combinedIndex];
                            else
                                throw new InvalidOperationException();
                        case 3:
                            if (left == LineWidth.None)
                                return CharsDoubleTRight[combinedIndex];
                            else if (top == LineWidth.None)
                                return CharsDoubleTBottom[combinedIndex];
                            else if (right == LineWidth.None)
                                return CharsDoubleTLeft[combinedIndex];
                            else if (bottom == LineWidth.None)
                                return CharsDoubleTTop[combinedIndex];
                            else
                                throw new InvalidOperationException();
                        case 4:
                            return CharsDoubleCross[combinedIndex];
                    }
                }

                throw GetCharException(lineChar);

                bool HasLineWidth(LineWidth width) => left == width || top == width || right == width || bottom == width;

                int CountLines(Func<LineWidth, bool> predicate) =>
                    (predicate(left) ? 1 : 0) + (predicate(top) ? 1 : 0)
                  + (predicate(right) ? 1 : 0) + (predicate(bottom) ? 1 : 0);
            }
        }

        private sealed class SimpleLineCharRenderer : LineCharRendererBase
        {
            public override char GetChar(LineChar lineChar)
            {
                LineWidth horizontal = LineWidthExts.Max(lineChar.Left, lineChar.Right);
                LineWidth vertical = LineWidthExts.Max(lineChar.Top, lineChar.Bottom);
                if (horizontal == LineWidth.None && vertical == LineWidth.None)
                    return '\0';
                if (horizontal != LineWidth.None && vertical != LineWidth.None)
                    return '+';
                if (horizontal != LineWidth.None)
                    return horizontal == LineWidth.Single ? '-' : '=';
                if (vertical != LineWidth.None)
                    return '|';
                throw GetCharException(lineChar);
            }
        }

        private sealed class CharLineCharRenderer : LineCharRendererBase
        {
            private readonly char _c;

            public CharLineCharRenderer(char c)
            {
                _c = c;
            }

            public override char GetChar(LineChar lineChar)
            {
                return lineChar.IsEmpty ? '\0' : _c;
            }
        }
    }
}