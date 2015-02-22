using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public delegate void ApplyColorMapDelegate (ref ConsoleChar c);

    public sealed class ConsoleBuffer : IConsoleBufferSource
    {
        private ILineCharRenderer _lineCharRenderer;
        private readonly List<ConsoleChar[]> _chars = new List<ConsoleChar[]>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Rect Clip { get; set; }
        public Vector Offset { get; set; }

        public ConsoleBuffer (int? width = null)
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

        public void DrawHorizontalLine (int x, int y, int w, ConsoleColor? color = null, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            int x1 = x, x2 = x + w;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                if (color != null)
                    charsLine[ix].ForegroundColor = color.Value;
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                charsLine[ix].LineWidthHorizontal = width | LineWidth.Single;
            }
        }

        public void DrawVerticalLine (int x, int y, int h, ConsoleColor? color = null, LineWidth width = LineWidth.Single)
        {
            if (width == LineWidth.None)
                return;
            int y1 = y, y2 = y + h;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                if (color != null)
                    charsLine[x].ForegroundColor = color.Value;
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                charsLine[x].LineWidthVertical = width | LineWidth.Single;
            }
        }

        public void DrawLine (Line line, ConsoleColor? color = null, LineWidth width = LineWidth.Single)
        {
            if (line.IsHorizontal)
                DrawHorizontalLine(line.X, line.Y, line.Width, color, width);
            else if (line.IsVertical)
                DrawVerticalLine(line.X, line.Y, line.Height, color, width);
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor? color = null, LineThickness? thickness = null)
        {
            if (thickness == null)
                thickness = new LineThickness(LineWidth.Single);
            DrawHorizontalLine(x, y, w, color, thickness.Value.Top);
            DrawHorizontalLine(x, y + h - 1, w, color, thickness.Value.Bottom);
            DrawVerticalLine(x, y, h, color, thickness.Value.Left);
            DrawVerticalLine(x + w - 1, y, h, color, thickness.Value.Right);
        }

        public void DrawRectangle (Rect rect, ConsoleColor? color = null, LineThickness? thickness = null)
        {
            DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, thickness);
        }

        public void DrawRectangle (int x, int y, int w, int h, ConsoleColor? color = null, LineWidth width = LineWidth.Single)
        {
            DrawHorizontalLine(x, y, w, color, width);
            DrawHorizontalLine(x, y + h - 1, w, color, width);
            DrawVerticalLine(x, y, h, color, width);
            DrawVerticalLine(x + w - 1, y, h, color, width);
        }

        public void DrawRectangle (Rect rect, ConsoleColor? color = null, LineWidth width = LineWidth.Single)
        {
            DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, width);
        }

        public void DrawString (int x, int y, ConsoleColor? color, string str)
        {
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + str.Length;
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                if (color != null)
                    charsLine[ix].ForegroundColor = color.Value;
                charsLine[ix].Char = str[ix - x];
            }
        }

        public void FillForegroundHorizontalLine (int x, int y, int w, ConsoleColor? color = null, [ValueProvider (ValueProviders.Chars)] char fill = '\0')
        {
            int x1 = x, x2 = x + w;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                if (color != null)
                    charsLine[ix].ForegroundColor = color.Value;
                charsLine[ix].Char = fill;
            }
        }

        public void FillForegroundVerticalLine (int x, int y, int h, ConsoleColor? color = null, [ValueProvider (ValueProviders.Chars)] char fill = '\0')
        {
            int y1 = y, y2 = y + h;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                if (color != null)
                    charsLine[x].ForegroundColor = color.Value;
                charsLine[x].Char = fill;
            }
        }

        public void FillForegroundLine (Line line, ConsoleColor? color = null, [ValueProvider (ValueProviders.Chars)] char fill = '\0')
        {
            if (line.IsHorizontal)
                FillForegroundHorizontalLine(line.X, line.Y, line.Width, color, fill);
            else if (line.IsVertical)
                FillForegroundVerticalLine(line.X, line.Y, line.Height, color, fill);
        }

        public void FillForegroundRectangle (int x, int y, int w, int h, ConsoleColor? color = null, [ValueProvider (ValueProviders.Chars)] char fill = '\0')
        {
            int y1 = y, y2 = y + h;
            for (int iy = y1; iy < y2; iy++)
                FillForegroundHorizontalLine(x, iy, w, color, fill);
        }

        public void FillForegroundRectangle (Rect rect, ConsoleColor? color = null, [ValueProvider (ValueProviders.Chars)] char fill = '\0')
        {
            FillForegroundRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, fill);
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

        public void FillBackgroundLine (Line line, ConsoleColor color)
        {
            if (line.IsHorizontal)
                FillBackgroundHorizontalLine(line.X, line.Y, line.Width, color);
            else if (line.IsVertical)
                FillBackgroundVerticalLine(line.X, line.Y, line.Height, color);
        }

        public void FillBackgroundRectangle (int x, int y, int w, int h, ConsoleColor color)
        {
            int y1 = y, y2 = y + h;
            for (int iy = y1; iy < y2; iy++)
                FillBackgroundHorizontalLine(x, iy, w, color);
        }

        public void FillBackgroundRectangle (Rect rect, ConsoleColor color)
        {
            FillBackgroundRectangle(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public void ResetClip ()
        {
            Clip = new Rect(0, 0, Width, Size.Infinity);
        }

        public void ApplyForegroundColorMap (int x, int y, int w, int h, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, colorMap,
                (ref ConsoleChar c) => c.ForegroundColor = colorMap[(int)c.ForegroundColor]);
        }

        public void ApplyForegroundColorMap (Rect rect, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap)
        {
            ApplyForegroundColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap);
        }

        public void ApplyBackgroundColorMap (int x, int y, int w, int h, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap)
        {
            ApplyColorMap(x, y, w, h, colorMap,
                (ref ConsoleChar c) => c.BackgroundColor = colorMap[(int)c.BackgroundColor]);
        }

        public void ApplyBackgroundColorMap (Rect rect, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap)
        {
            ApplyBackgroundColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap);
        }

        public void ApplyColorMap (int x, int y, int w, int h, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap,
            ApplyColorMapDelegate processChar)
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
                ConsoleChar[] charsLine = GetLine(iy);
                for (int ix = x1; ix < x2; ix++)
                    processChar(ref charsLine[ix]);
            }
        }

        public void ApplyColorMap (Rect rect, [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] colorMap,
            ApplyColorMapDelegate processChar)
        {
            ApplyColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap, processChar);
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

        private ConsoleBuffer OffsetX (ref int x)
        {
            x += Offset.X;
            return this;
        }

        private ConsoleBuffer OffsetY (ref int y)
        {
            y += Offset.Y;
            return this;
        }

        private ConsoleChar[] GetLine (int y)
        {
            while (_chars.Count <= y)
                _chars.Add(new ConsoleChar[Width]);
            if (Height <= y)
                Height = y + 1;
            return _chars[y];
        }

        ConsoleChar[] IConsoleBufferSource.GetLine (int y)
        {
            return GetLine(y);
        }

        private ConsoleChar? GetChar (int x, int y)
        {
            if (!new Rect(0, 0, Width, Height).Contains(x, y))
                return null;
            return GetLine(y)[x];
        }

        ConsoleChar? IConsoleBufferSource.GetChar (int x, int y)
        {
            return GetChar(x, y);
        }

        char IConsoleBufferSource.GetLineChar (int x, int y)
        {
            ConsoleChar? chr = GetChar(x, y);
            if (chr == null)
                return '\0';
            ConsoleChar? chrLeft = GetChar(x - 1, y);
            ConsoleChar? chrTop = GetChar(x, y - 1);
            ConsoleChar? chrRight = GetChar(x + 1, y);
            ConsoleChar? chrBottom = GetChar(x, y + 1);
            return LineCharRenderer.GetChar(chr.Value.LineChar,
                chrLeft != null ? chrLeft.Value.LineChar : LineChar.None,
                chrTop != null ? chrTop.Value.LineChar : LineChar.None,
                chrRight != null ? chrRight.Value.LineChar : LineChar.None,
                chrBottom != null ? chrBottom.Value.LineChar : LineChar.None);
        }

        char IConsoleBufferSource.SafeChar (char chr)
        {
            if (chr < ' ')
                return ' ';
            if (chr == Chars.NoBreakHyphen || chr == Chars.SoftHyphen)
                return '-';
            if (chr == Chars.NoBreakSpace || chr == Chars.ZeroWidthSpace)
                return ' ';
            return chr;
        }
    }
}