using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderBuffer
    {
        private const string ColorMapsProvider = "Alba.CsConsoleFormat.ColorMaps";
        private const string CharsProvider = "Alba.CsConsoleFormat.Chars";

        private ILineCharRenderer _lineCharRenderer;
        private readonly List<ConsoleColor[]> _foreColors = new List<ConsoleColor[]>();
        private readonly List<ConsoleColor[]> _backColors = new List<ConsoleColor[]>();
        private readonly List<LineChar[]> _lineChars = new List<LineChar[]>();
        private readonly List<char[]> _chars = new List<char[]>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Rect Clip { get; set; }
        public Vector Offset { get; set; }

        public ConsoleRenderBuffer (int? width = null)
        {
            _lineCharRenderer = CsConsoleFormat.LineCharRenderer.Box;
            Width = width ?? Console.BufferWidth;
            Height = 0;
            ResetClip();
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

        public void DrawHorizontalLine (int y, int x1, int x2, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            LineChar[] lineCharsLine = GetLine(_lineChars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                ModifyLineChar(ref lineCharsLine[ix], width, false);
            }
        }

        public void DrawVerticalLine (int x, int y1, int y2, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                GetLine(_foreColors, iy)[x] = color;
                ModifyLineChar(ref GetLine(_lineChars, iy)[x], width, true);
            }
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor color, LineThickness thickness)
        {
            DrawHorizontalLine(y, x, x + w, color, thickness.Top);
            DrawHorizontalLine(y + h - 1, x, x + w, color, thickness.Bottom);
            DrawVerticalLine(x, y, y + h, color, thickness.Left);
            DrawVerticalLine(x + w - 1, y, y + h, color, thickness.Right);
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            DrawHorizontalLine(y, x, x + w, color, width);
            DrawHorizontalLine(y + h - 1, x, x + w, color, width);
            DrawVerticalLine(x, y, y + h, color, width);
            DrawVerticalLine(x + w - 1, y, y + h, color, width);
        }

        public void DrawString (int x, int y, ConsoleColor color, string str)
        {
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + str.Length;
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            char[] charLine = GetLine(_chars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                charLine[ix] = str[ix - x];
            }
        }

        public void FillForegroundHorizontalLine (int y, int x1, int x2, ConsoleColor color,
            [ValueProvider (CharsProvider)] char fill)
        {
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleColor[] foreColorsLine = GetLine(_foreColors, y);
            char[] charLine = GetLine(_chars, y);
            for (int ix = x1; ix < x2; ix++) {
                foreColorsLine[ix] = color;
                charLine[ix] = fill;
            }
        }

        public void FillForegroundVerticalLine (int x, int y1, int y2, ConsoleColor color,
            [ValueProvider (CharsProvider)] char fill)
        {
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                GetLine(_foreColors, iy)[x] = color;
                GetLine(_chars, iy)[x] = fill;
            }
        }

        public void FillForegroundRectangle (int x, int y, int w, int h, ConsoleColor color,
            [ValueProvider (CharsProvider)] char fill)
        {
            for (int iy = y; iy < y + h; iy++)
                FillForegroundHorizontalLine(iy, x, x + w, color, fill);
        }

        public void FillBackgroundHorizontalLine (int y, int x1, int x2, ConsoleColor color)
        {
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleColor[] backColorsLine = GetLine(_backColors, y);
            for (int ix = x1; ix < x2; ix++)
                backColorsLine[ix] = color;
        }

        public void FillBackgroundVerticalLine (int x, int y1, int y2, ConsoleColor color)
        {
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++)
                GetLine(_backColors, iy)[x] = color;
        }

        public void FillBackgroundRectangle (int x, int y, int w, int h, ConsoleColor color)
        {
            for (int iy = y; iy < y + h; iy++)
                FillBackgroundHorizontalLine(iy, x, x + w, color);
        }

        public void ResetClip ()
        {
            Clip = new Rect(0, 0, Width, int.MaxValue);
        }

        public void ApplyForegroundColorMap (int x, int y, int w, int h,
            [ValueProvider (ColorMapsProvider)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, _foreColors, colorMap);
        }

        public void ApplyBackgroundColorMap (int x, int y, int w, int h,
            [ValueProvider (ColorMapsProvider)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, _backColors, colorMap);
        }

        private void ApplyColorMap (int x, int y, int w, int h, List<ConsoleColor[]> colors,
            [ValueProvider (ColorMapsProvider)] ConsoleColor[] colorMap)
        {
            if (colorMap == null)
                throw new ArgumentNullException("colorMap");
            if (colorMap.Length != ColorMaps.ConsoleColorCount)
                throw new ArgumentException("colorMap must contain 16 elements corresponding to each ConsoleColor.");
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + w, y1 = y, y2 = y + h;
            if (!ClipRectangle(ref x1, ref y1, ref x2, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleColor[] colorsLine = GetLine(colors, iy);
                for (int ix = x1; ix < x2; ix++)
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

        private bool ClipHorizontalLine (int y, ref int x1, ref int x2)
        {
            if (!Clip.IntersectsHorizontalLine(y))
                return false;
            x1 = Math.Max(x1, Clip.X);
            x2 = Math.Min(x2, Clip.Right);
            return true;
        }

        private bool ClipVerticalLine (int x, ref int y1, ref int y2)
        {
            if (!Clip.IntersectsVerticalLine(x))
                return false;
            y1 = Math.Max(y1, Clip.Y);
            y2 = Math.Min(y2, Clip.Bottom);
            return true;
        }

        private bool ClipRectangle (ref int x1, ref int y1, ref int x2, ref int y2)
        {
            x1 = Math.Max(x1, Clip.X);
            x2 = Math.Min(x2, Clip.Right);
            y1 = Math.Max(y1, Clip.Y);
            y2 = Math.Min(y2, Clip.Bottom);
            return x1 < x2 && y1 < y2;
        }

        private ConsoleRenderBuffer OffsetX (ref int x)
        {
            x += Offset.X;
            return this;
        }

        private ConsoleRenderBuffer OffsetY (ref int y)
        {
            y += Offset.Y;
            return this;
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