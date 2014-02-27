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

        public void DrawRectangle (ConsoleColor color, int x, int y, int w, int h)
        {
            for (int iy = y; iy < y + h; iy++) {
                ConsoleColor[] line = GetLine(_backColors, iy);
                for (int ix = x; ix < x + w; ix++)
                    line[ix] = color;
            }
        }

        public void RenderToConsole ()
        {
            ConsoleColor currentForeColor = Console.ForegroundColor, oldForeColor = currentForeColor;
            ConsoleColor currentBackColor = Console.BackgroundColor, oldBackColor = currentBackColor;
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
                    char chr = charsLine != null ? charsLine[ix] : ' ';
                    Console.Write(chr);
                }
            }
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
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

        [Flags]
        private enum LineChar
        {
            Horizontal = 1 << 0,
            HorizontalWide = 1 << 1,
            Vertical = 1 << 2,
            VerticalWide = 1 << 3,
        }
    }
}