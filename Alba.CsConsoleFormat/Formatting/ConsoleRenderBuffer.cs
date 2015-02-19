using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderBuffer
    {
        private delegate void ApplyColorMapDelegate (ref ConsoleChar c);

        private const string ColorMapsProvider = "Alba.CsConsoleFormat.ColorMaps";
        private const string CharsProvider = "Alba.CsConsoleFormat.Chars";

        private ILineCharRenderer _lineCharRenderer;
        private readonly List<ConsoleChar[]> _chars = new List<ConsoleChar[]>();

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

        public void DrawHorizontalLine (int x, int y, int w, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            int x1 = x, x2 = x + w;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                charsLine[ix].ForegroundColor = color;
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                charsLine[ix].LineWidthHorizontal = width | LineWidth.Single;
            }
        }

        public void DrawVerticalLine (int x, int y, int h, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            int y1 = y, y2 = y + h;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                charsLine[x].ForegroundColor = color;
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                charsLine[x].LineWidthVertical = width | LineWidth.Single;
            }
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor color, LineThickness thickness)
        {
            DrawHorizontalLine(x, y, w, color, thickness.Top);
            DrawHorizontalLine(x, y + h - 1, w, color, thickness.Bottom);
            DrawVerticalLine(x, y, h, color, thickness.Left);
            DrawVerticalLine(x + w - 1, y, h, color, thickness.Right);
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor color, LineWidth width = LineWidth.Single)
        {
            DrawHorizontalLine(x, y, w, color, width);
            DrawHorizontalLine(x, y + h - 1, w, color, width);
            DrawVerticalLine(x, y, h, color, width);
            DrawVerticalLine(x + w - 1, y, h, color, width);
        }

        public void DrawString (int x, int y, ConsoleColor color, string str)
        {
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + str.Length;
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                charsLine[ix].ForegroundColor = color;
                charsLine[ix].Char = str[ix - x];
            }
        }

        public void FillForegroundHorizontalLine (int x, int y, int w, ConsoleColor color, [ValueProvider (CharsProvider)] char fill)
        {
            int x1 = x, x2 = x + w;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                charsLine[ix].ForegroundColor = color;
                charsLine[ix].Char = fill;
            }
        }

        public void FillForegroundVerticalLine (int x, int y, int h, ConsoleColor color, [ValueProvider (CharsProvider)] char fill)
        {
            int y1 = y, y2 = y + h;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                charsLine[x].ForegroundColor = color;
                charsLine[x].Char = fill;
            }
        }

        public void FillForegroundRectangle (int x, int y, int w, int h, ConsoleColor color, [ValueProvider (CharsProvider)] char fill)
        {
            int y1 = y, y2 = y + h;
            for (int iy = y1; iy < y2; iy++)
                FillForegroundHorizontalLine(x, iy, w, color, fill);
        }

        public void FillBackgroundHorizontalLine (int x, int y, int w, ConsoleColor color)
        {
            int x1 = x, x2 = x + w;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++)
                charsLine[ix].BackgroundColor = color;
        }

        public void FillBackgroundVerticalLine (int x, int y, int h, ConsoleColor color)
        {
            int y1 = y, y2 = y + h;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                charsLine[x].BackgroundColor = color;
            }
        }

        public void FillBackgroundRectangle (int x, int y, int w, int h, ConsoleColor color)
        {
            int y1 = y, y2 = y + h;
            for (int iy = y1; iy < y2; iy++)
                FillBackgroundHorizontalLine(x, iy, w, color);
        }

        public void ResetClip ()
        {
            Clip = new Rect(0, 0, Width, Size.Infinity);
        }

        public void ApplyForegroundColorMap (int x, int y, int w, int h, [ValueProvider (ColorMapsProvider)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, colorMap,
                (ref ConsoleChar c) => c.ForegroundColor = colorMap[(int)c.ForegroundColor]);
        }

        public void ApplyBackgroundColorMap (int x, int y, int w, int h, [ValueProvider (ColorMapsProvider)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, colorMap,
                (ref ConsoleChar c) => c.BackgroundColor = colorMap[(int)c.BackgroundColor]);
        }

        private void ApplyColorMap (int x, int y, int w, int h, ConsoleColor[] colorMap, ApplyColorMapDelegate processChar)
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
                ConsoleChar[] charsLine = GetLine(y);
                for (int ix = x1; ix < x2; ix++)
                    processChar(ref charsLine[ix]);
            }
        }

        public void RenderToConsole ()
        {
            ConsoleColor currentForeColor = Console.ForegroundColor, oldForeColor = currentForeColor;
            ConsoleColor currentBackColor = Console.BackgroundColor, oldBackColor = currentBackColor;
            try {
                for (int iy = 0; iy < Height; iy++) {
                    ConsoleChar[] charsLine = GetLine(iy);

                    for (int ix = 0; ix < Width; ix++) {
                        ConsoleColor foreColor = charsLine[ix].ForegroundColor;
                        if (foreColor != currentForeColor)
                            Console.ForegroundColor = currentForeColor = foreColor;
                        ConsoleColor backColor = charsLine[ix].BackgroundColor;
                        if (backColor != currentBackColor)
                            Console.BackgroundColor = currentBackColor = backColor;
                        LineChar lineChr = charsLine[ix].LineChar;
                        char chr = charsLine[ix].Char;
                        if (!lineChr.IsEmpty() && chr == '\0')
                            chr = LineCharRenderer.GetChar(lineChr,
                                GetLineCharAt(ix - 1, iy), GetLineCharAt(ix, iy - 1), GetLineCharAt(ix + 1, iy), GetLineCharAt(ix, iy + 1));
                        if (chr < ' ')
                            chr = ' ';
                        else if (chr == Chars.NoBreakHyphen || chr == Chars.SoftHyphen)
                            chr = '-';
                        else if (chr == Chars.NoBreakSpace || chr == Chars.ZeroWidthSpace)
                            chr = ' ';
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
            return GetLine(y)[x].LineChar;
        }

        private ConsoleChar[] GetLine (int y)
        {
            while (_chars.Count <= y)
                _chars.Add(new ConsoleChar[Width]);
            if (Height <= y)
                Height = y + 1;
            return _chars[y];
        }
    }
}