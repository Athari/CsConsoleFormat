using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderBuffer
    {
        private readonly List<ConsoleColor[]> _foreColors = new List<ConsoleColor[]>();
        private readonly List<ConsoleColor[]> _backColors = new List<ConsoleColor[]>();
        private readonly List<LineChar[]> _lineChars = new List<LineChar[]>();
        private readonly List<char[]> _chars = new List<char[]>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public ConsoleRenderBuffer (int? width = null)
        {
            Width = width ?? Console.BufferWidth;
            Height = 0;
        }

        public void DrawHorizontalLine (ConsoleColor color, int x1, int y, int x2, LineWidth width = LineWidth.Single)
        {
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            LineChar[] lineCharsLine = GetLine(_lineChars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                ModifyLineChar(ref lineCharsLine[ix], width, false);
            }
        }

        public void DrawVerticalLine (ConsoleColor color, int x, int y1, int y2, LineWidth width = LineWidth.Single)
        {
            for (int iy = y1; iy < y2; iy++) {
                GetLine(_foreColors, iy)[x] = color;
                ModifyLineChar(ref GetLine(_lineChars, iy)[x], width, true);
            }
        }

        public void DrawRectangle (ConsoleColor color, int x, int y, int w, int h, LineWidth width = LineWidth.Single)
        {
            DrawHorizontalLine(color, x, y, x + w, width);
            DrawHorizontalLine(color, x, y + h - 1, x + w, width);
            DrawVerticalLine(color, x, y, y + h, width);
            DrawVerticalLine(color, x + w - 1, y, y + h, width);
        }

        public void DrawString (ConsoleColor color, int x, int y, string str)
        {
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            char[] charLine = GetLine(_chars, y);
            for (int i = 0; i < str.Length; i++) {
                foreColorsLine[x + i] = color;
                charLine[x + i] = str[i];
            }
        }

        public void FillHorizontalLine (ConsoleColor color, int x1, int y, int x2)
        {
            ConsoleColor[] backColorsLine = GetLine(_backColors, y);
            for (int ix = x1; ix < x2; ix++)
                backColorsLine[ix] = color;
        }

        public void FillVerticalLine (ConsoleColor color, int x, int y1, int y2)
        {
            for (int iy = y1; iy < y2; iy++)
                GetLine(_backColors, iy)[x] = color;
        }

        public void FillRectangle (ConsoleColor color, int x, int y, int w, int h)
        {
            for (int iy = y; iy < y + h; iy++)
                FillHorizontalLine(color, x, iy, x + w);
        }

        public void RenderToConsole ()
        {
            ILineCharRenderer lineRenderer = LineCharRenderer.Box;

            ConsoleColor currentForeColor = Console.ForegroundColor, oldForeColor = currentForeColor;
            ConsoleColor currentBackColor = Console.BackgroundColor, oldBackColor = currentBackColor;
            try {
                for (int iy = 0; iy < Height; iy++) {
                    ConsoleColor[] foreColorsLine = GetLineNullable(_foreColors, iy);
                    ConsoleColor[] backColorsLine = GetLineNullable(_backColors, iy);
                    LineChar[] lineCharsLine = GetLineNullable(_lineChars, iy);
                    char[] charsLine = GetLineNullable(_chars, iy);

                    for (int ix = 0; ix < Width; ix++) {
                        ConsoleColor? foreColor = foreColorsLine != null ? foreColorsLine[ix] : (ConsoleColor?)null;
                        if (foreColor.HasValue && foreColor.Value != currentForeColor)
                            Console.ForegroundColor = currentForeColor = foreColor.Value;
                        ConsoleColor? backColor = backColorsLine != null ? backColorsLine[ix] : (ConsoleColor?)null;
                        if (backColor.HasValue && backColor.Value != currentBackColor)
                            Console.BackgroundColor = currentBackColor = backColor.Value;
                        LineChar? lineChr = lineCharsLine != null ? lineCharsLine[ix] : (LineChar?)null;
                        char chr = charsLine != null ? charsLine[ix] : '\0';
                        if (lineChr.HasValue && !lineChr.Value.IsEmpty() && chr == '\0')
                            chr = lineRenderer.GetChar(lineChr.Value,
                                GetLineCharAt(ix - 1, iy), GetLineCharAt(ix, iy - 1), GetLineCharAt(ix + 1, iy), GetLineCharAt(ix, iy + 1));
                        Console.Write(chr);
                    }
                }
            }
            finally {
                Console.ForegroundColor = oldForeColor;
                Console.BackgroundColor = oldBackColor;
            }
        }

        private LineChar GetLineCharAt (int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return LineChar.None;
            LineChar[] lineCharsLine = GetLineNullable(_lineChars, y);
            return lineCharsLine != null ? lineCharsLine[x] : LineChar.None;
        }

        private T[] GetLine<T> (List<T[]> lines, int y)
        {
            while (lines.Count <= y)
                lines.Add(new T[Width]);
            if (Height <= y)
                Height = y + 1;
            return lines[y];
        }

        private T[] GetLineNullable<T> (List<T[]> lines, int y)
        {
            return y < lines.Count ? lines[y] : null;
        }

        private static void ModifyLineChar (ref LineChar chr, LineWidth width, bool isVertical)
        {
            chr = isVertical
                ? (chr & LineChar.MaskHorizontal) | LineChar.Vertical | (width == LineWidth.Wide ? LineChar.VerticalWide : 0)
                : (chr & LineChar.MaskVertical) | LineChar.Horizontal | (width == LineWidth.Wide ? LineChar.HorizontalWide : 0);
        }
    }
}