using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderBuffer
    {
        private ILineCharRenderer _lineCharRenderer;
        private readonly List<ConsoleColor[]> _foreColors = new List<ConsoleColor[]>();
        private readonly List<ConsoleColor[]> _backColors = new List<ConsoleColor[]>();
        private readonly List<LineChar[]> _lineChars = new List<LineChar[]>();
        private readonly List<char[]> _chars = new List<char[]>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public ConsoleRenderBuffer (int? width = null)
        {
            _lineCharRenderer = CsConsoleFormat.LineCharRenderer.Box;
            Width = width ?? Console.BufferWidth;
            Height = 0;
        }

        public ILineCharRenderer LineCharRenderer
        {
            get { return _lineCharRenderer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _lineCharRenderer = value;
            }
        }

        public void DrawHorizontalLine (int x1, int y, int x2, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            LineChar[] lineCharsLine = GetLine(_lineChars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                ModifyLineChar(ref lineCharsLine[ix], width, false);
            }
        }

        public void DrawVerticalLine (int x, int y1, int y2, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            for (int iy = y1; iy < y2; iy++) {
                GetLine(_foreColors, iy)[x] = color;
                ModifyLineChar(ref GetLine(_lineChars, iy)[x], width, true);
            }
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            DrawHorizontalLine(x, y, x + w, color, width);
            DrawHorizontalLine(x, y + h - 1, x + w, color, width);
            DrawVerticalLine(x, y, y + h, color, width);
            DrawVerticalLine(x + w - 1, y, y + h, color, width);
        }

        public void DrawString (int x, int y, ConsoleColor color, string str)
        {
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            char[] charLine = GetLine(_chars, y);
            for (int i = 0; i < str.Length; i++) {
                foreColorsLine[x + i] = color;
                charLine[x + i] = str[i];
            }
        }

        public void FillForegroundHorizontalLine (int x1, int y, int x2, ConsoleColor color, char fill)
        {
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            char[] charLine = GetLine(_chars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                charLine[ix] = fill;
            }
        }

        public void FillForegroundVerticalLine (int x, int y1, int y2, ConsoleColor color, char fill)
        {
            for (int iy = y1; iy < y2; iy++) {
                GetLine(_foreColors, iy)[x] = color;
                GetLine(_chars, iy)[x] = fill;
            }
        }

        public void FillForegroundRectangle (int x, int y, int w, int h, ConsoleColor color, char fill)
        {
            for (int iy = y; iy < y + h; iy++)
                FillForegroundHorizontalLine(x, iy, x + w, color, fill);
        }

        public void FillBackgroundHorizontalLine (int x1, int y, int x2, ConsoleColor color)
        {
            ConsoleColor[] backColorsLine = GetLine(_backColors, y);
            for (int ix = x1; ix < x2; ix++)
                backColorsLine[ix] = color;
        }

        public void FillBackgroundVerticalLine (int x, int y1, int y2, ConsoleColor color)
        {
            for (int iy = y1; iy < y2; iy++)
                GetLine(_backColors, iy)[x] = color;
        }

        public void FillBackgroundRectangle (int x, int y, int w, int h, ConsoleColor color)
        {
            for (int iy = y; iy < y + h; iy++)
                FillBackgroundHorizontalLine(x, iy, x + w, color);
        }

        public void ApplyForegroundColorMap (int x, int y, int w, int h, ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, _foreColors, colorMap);
        }

        public void ApplyBackgroundColorMap (int x, int y, int w, int h, ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, _backColors, colorMap);
        }

        private void ApplyColorMap (int x, int y, int w, int h, List<ConsoleColor[]> colors, ConsoleColor[] colorMap)
        {
            if (colorMap == null)
                throw new ArgumentNullException("colorMap");
            if (colorMap.Length != ColorMaps.ConsoleColorCount)
                throw new ArgumentException("colorMap must contain 16 elements corresponding to each ConsoleColor.");
            for (int iy = y; iy < y + h; iy++) {
                ConsoleColor[] colorsLine = GetLine(colors, iy);
                for (int ix = x; ix < x + w; ix++)
                    colorsLine[ix] = colorMap[(int)colorsLine[ix]];
            }
        }

        public void RenderToConsole ()
        {
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
                            chr = LineCharRenderer.GetChar(lineChr.Value,
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