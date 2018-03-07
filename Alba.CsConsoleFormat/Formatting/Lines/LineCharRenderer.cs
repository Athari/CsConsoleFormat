using System;
using static Alba.CsConsoleFormat.LineWidthExts;

namespace Alba.CsConsoleFormat
{
    public static class LineCharRenderer
    {
        public static readonly ILineCharRenderer Box = new BoxLineCharRenderer(supportExtended: false);
        public static readonly ILineCharRenderer BoxExtended = new BoxLineCharRenderer(supportExtended: true);
        public static readonly ILineCharRenderer None = new CharLineCharRenderer('\0');
        public static readonly ILineCharRenderer Simple = new SimpleLineCharRenderer();
        public static ILineCharRenderer Char(char c) => new CharLineCharRenderer(c);

        private abstract class LineCharRendererBase : ILineCharRenderer
        {
            public abstract char GetChar(LineChar lineChar);

            protected static Exception GetCharException(LineChar lineChar)
            {
                return new NotSupportedException($"Line char '{lineChar}' not supported.");
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

            private static readonly char[] CharsHeavySideSingle = { '╴', '╵', '╶', '╷' };
            private static readonly char[] CharsHeavySideHeavy = { '╸', '╹', '╺', '╻' };
            private static readonly char[] CharsHeavyHorizontal = { '─', '━' };
            private static readonly char[] CharsHeavyHorizontalUneven = { '╾', '╼' };
            private static readonly char[] CharsHeavyVertical = { '│', '┃' };
            private static readonly char[] CharsHeavyVerticalUneven = { '╿', '╽' };
            private static readonly char[] CharsHeavyCornerTopLeft = { '┘', '┙', '┚', '┛' };
            private static readonly char[] CharsHeavyCornerTopRight = { '└', '┕', '┖', '┗' };
            private static readonly char[] CharsHeavyCornerBottomLeft = { '┐', '┑', '┒', '┓' };
            private static readonly char[] CharsHeavyCornerBottomRight = { '┌', '┍', '┎', '┏' };
            private static readonly char[] CharsHeavyTLeft = { '┤', '┥', '┨', '┫' };
            private static readonly char[] CharsHeavyTLeftUneven = { '┦', '┧', '┩', '┪' };
            private static readonly char[] CharsHeavyTTop = { '┴', '┷', '┸', '┻' };
            private static readonly char[] CharsHeavyTTopUneven = { '┵', '┶', '┹', '┺' };
            private static readonly char[] CharsHeavyTRight = { '├', '┝', '┠', '┣' };
            private static readonly char[] CharsHeavyTRightUneven = { '┞', '┟', '┡', '┢' };
            private static readonly char[] CharsHeavyTBottom = { '┬', '┯', '┰', '┳' };
            private static readonly char[] CharsHeavyTBottomUneven = { '┭', '┮', '┱', '┲' };
            private static readonly char[] CharsHeavyCross = { '┼', '┿', '╂', '╋' };
            private static readonly char[] CharsHeavyCrossUnevenSide = { '┽', '╀', '┾', '╁' };
            private static readonly char[] CharsHeavyCrossUnevenCorner = { '╃', '╄', '╅', '╆' };
            private static readonly char[] CharsHeavyCrossUnevenT = { '╊', '╈', '╉', '╇' };

            private readonly bool _supportExtended;

            public BoxLineCharRenderer(bool supportExtended)
            {
                _supportExtended = supportExtended;
            }

            public override char GetChar(LineChar lineChar)
            {
                const LineWidth None = LineWidth.None;
                const LineWidth Single = LineWidth.Single;
                const LineWidth Double = LineWidth.Double;
                const LineWidth Heavy = LineWidth.Heavy;

                LineWidth left = lineChar.Left;
                LineWidth top = lineChar.Top;
                LineWidth right = lineChar.Right;
                LineWidth bottom = lineChar.Bottom;

                bool hasSingle = HasLineWidth(Single);
                bool hasDouble = HasLineWidth(Double);
                bool hasHeavy = HasLineWidth(Heavy);
                int lineCount = CountAllLines();

                if (hasHeavy && hasDouble)
                    throw new InvalidOperationException("Heavy and Double line widths can't be combined.");

                bool isHor = left != None && right != None;
                bool isVer = top != None && bottom != None;
                int horIndex = EvenIndex(left, right);
                int verIndex = EvenIndex(top, bottom);
                int combinedIndex = CombineIndex(verIndex, horIndex);

                return !hasSingle && !hasHeavy && !hasDouble ? '\0' : hasHeavy ? GetHeavyChar() : GetDoubleChar();

                bool HasLineWidth(LineWidth width) => left == width || top == width || right == width || bottom == width;
                int CountAllLines() => (left != None ? 1 : 0) + (top != None ? 1 : 0) + (right != None ? 1 : 0) + (bottom != None ? 1 : 0);
                int CountLines(LineWidth width) => (left == width ? 1 : 0) + (top == width ? 1 : 0) + (right == width ? 1 : 0) + (bottom == width ? 1 : 0);
                int EvenIndex(LineWidth a, LineWidth b) => Max(a, b) == Single ? 0 : 1;
                int UnevenIndex(LineWidth a, LineWidth b) => a != None && b != None && a != b ? (a == Heavy ? 0 : 1) : -1;
                int CombineIndex(int index1, int index2) => index1 * 2 + index2;
                int FirstIndex(LineWidth width) => left == width ? 0 : top == width ? 1 : right == width ? 2 : bottom == width ? 3 : -1;

                char GetCornerChar(
                    char[] charsCornerTopLeft, char[] charsCornerTopRight,
                    char[] charsCornerBottomLeft, char[] charsCornerBottomRight)
                {
                    if (top != None && left != None)
                        return charsCornerTopLeft[combinedIndex];
                    else if (top != None && right != None)
                        return charsCornerTopRight[combinedIndex];
                    else if (bottom != None && left != None)
                        return charsCornerBottomLeft[combinedIndex];
                    else if (bottom != None && right != None)
                        return charsCornerBottomRight[combinedIndex];
                    throw GetCharException(lineChar);
                }

                char GetEvenTChar(char[] charsTRight, char[] charsTBottom, char[] charsTLeft, char[] charsTTop)
                {
                    if (left == None)
                        return charsTRight[combinedIndex];
                    else if (top == None)
                        return charsTBottom[combinedIndex];
                    else if (right == None)
                        return charsTLeft[combinedIndex];
                    else if (bottom == None)
                        return charsTTop[combinedIndex];
                    throw GetCharException(lineChar);
                }

                char GetHeavyChar()
                {
                    int lineCountHeavy = CountLines(Heavy);
                    int horUnevenIndex = UnevenIndex(left, right);
                    int verUnevenIndex = UnevenIndex(top, bottom);
                    int firstSingleIndex = FirstIndex(Single);
                    int firstHeavyIndex = FirstIndex(Heavy);
                    bool isEven = horUnevenIndex == -1 && verUnevenIndex == -1;

                    switch (lineCount) {
                        case 1:
                            return firstSingleIndex != -1 ? CharsHeavySideSingle[firstSingleIndex] : CharsHeavySideHeavy[firstHeavyIndex];
                        case 2:
                            if (isHor)
                                return horUnevenIndex == -1 ? CharsHeavyHorizontal[horIndex] : CharsHeavyHorizontalUneven[horUnevenIndex];
                            else if (isVer)
                                return verUnevenIndex == -1 ? CharsHeavyVertical[verIndex] : CharsHeavyVerticalUneven[horUnevenIndex];
                            else
                                return GetCornerChar(
                                    CharsHeavyCornerTopLeft, CharsHeavyCornerTopRight,
                                    CharsHeavyCornerBottomLeft, CharsHeavyCornerBottomRight);
                        case 3:
                            if (isEven)
                                return GetEvenTChar(CharsHeavyTRight, CharsHeavyTBottom, CharsHeavyTLeft, CharsHeavyTTop);
                            else if (left == None)
                                return CharsHeavyTRightUneven[CombineIndex(right == Single ? 1 : 0, verUnevenIndex)];
                            else if (top == None)
                                return CharsHeavyTBottomUneven[CombineIndex(bottom == Single ? 1 : 0, horUnevenIndex)];
                            else if (right == None)
                                return CharsHeavyTLeftUneven[CombineIndex(left == Single ? 1 : 0, verUnevenIndex)];
                            else if (bottom == None)
                                return CharsHeavyTTopUneven[CombineIndex(top == Single ? 1 : 0, horUnevenIndex)];
                            break;
                        case 4:
                            if (isEven)
                                return CharsHeavyCross[combinedIndex];
                            switch (lineCountHeavy) {
                                case 1:
                                    return CharsHeavyCrossUnevenSide[firstHeavyIndex];
                                case 2:
                                    return CharsHeavyCrossUnevenCorner[CombineIndex(verUnevenIndex, horUnevenIndex)];
                                case 3:
                                    return CharsHeavyCrossUnevenT[firstSingleIndex];
                            }
                            break;
                    }
                    throw GetCharException(lineChar);
                }

                char GetDoubleChar()
                {
                    int firstSingleIndex = FirstIndex(Single);

                    switch (lineCount) {
                        case 1:
                            return _supportExtended
                                ? CharsHeavySideSingle[firstSingleIndex]
                                : left != None || right != None ? CharsDoubleHorizontal[horIndex] : CharsDoubleVertical[verIndex];
                        case 2:
                            if (isHor)
                                return CharsDoubleHorizontal[horIndex];
                            else if (isVer)
                                return CharsDoubleVertical[verIndex];
                            else
                                return GetCornerChar(
                                    CharsDoubleCornerTopLeft, CharsDoubleCornerTopRight,
                                    CharsDoubleCornerBottomLeft, CharsDoubleCornerBottomRight);
                        case 3:
                            return GetEvenTChar(CharsDoubleTRight, CharsDoubleTBottom, CharsDoubleTLeft, CharsDoubleTTop);
                        case 4:
                            return CharsDoubleCross[combinedIndex];
                    }
                    throw GetCharException(lineChar);
                }
            }
        }

        private sealed class SimpleLineCharRenderer : LineCharRendererBase
        {
            public override char GetChar(LineChar lineChar)
            {
                LineWidth horizontal = Max(lineChar.Left, lineChar.Right);
                LineWidth vertical = Max(lineChar.Top, lineChar.Bottom);
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