using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.Framework.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public delegate void ApplyColorMapCallback(ref ConsoleChar c);

    public sealed class ConsoleBuffer : IConsoleBufferSource
    {
        private ILineCharRenderer _lineCharRenderer;
        private readonly List<ConsoleChar[]> _chars = new List<ConsoleChar[]>();

        public int Width { get; }
        public int Height { get; private set; }
        public Rect Clip { get; set; }
        public Vector Offset { get; set; }

        public ConsoleBuffer(int? width = null)
        {
            _lineCharRenderer = CsConsoleFormat.LineCharRenderer.Box;
            Width = width ?? Console.BufferWidth;
            Height = 0;
            ResetClip();
        }

        [NotNull]
        public ILineCharRenderer LineCharRenderer
        {
            get => _lineCharRenderer;
            set => _lineCharRenderer = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void DrawHorizontalLine(int x, int y, int width, ConsoleColor? color = null, LineWidth lineWidth = LineWidth.Single,
            DrawLineFlags flags = DrawLineFlags.CapCenter)
        {
            int x1 = x, x2 = x + width;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            int unclippedx1 = x1, unclippedx2 = x2;
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;

            // Draw line
            ConsoleChar[] charsLine = GetLine(y);
            if (color != null) {
                for (int ix = x1; ix < x2; ix++)
                    charsLine[ix].ForegroundColor = color.Value;
            }
            for (int ix = x1 + 1; ix < x2 - 1; ix++) {
                LineChar lineChar = charsLine[ix].LineChar;
                lineChar.SetHorizontal(lineWidth);
                charsLine[ix].LineChar = lineChar;
            }

            // Cap start
            LineChar lineCharStart = charsLine[x1].LineChar;
            if (x1 != unclippedx1)
                lineCharStart.SetHorizontal(lineWidth);
            else if (flags.HasCapStartFull())
                lineCharStart.SetHorizontal(lineWidth);
            else if (flags.HasCapStartCenter())
                lineCharStart.Right = lineWidth;
            charsLine[x1].LineChar = lineCharStart;

            // Cap end
            LineChar lineCharEnd = charsLine[x2 - 1].LineChar;
            if (x2 != unclippedx2)
                lineCharEnd.SetHorizontal(lineWidth);
            else if (flags.HasCapEndFull())
                lineCharEnd.SetHorizontal(lineWidth);
            else if (flags.HasCapEndCenter())
                lineCharEnd.Left = lineWidth;
            charsLine[x2 - 1].LineChar = lineCharEnd;
        }

        public void DrawVerticalLine(int x, int y, int height, ConsoleColor? color = null, LineWidth lineWidth = LineWidth.Single,
            DrawLineFlags flags = DrawLineFlags.CapCenter)
        {
            if (lineWidth == LineWidth.None)
                return;

            int y1 = y, y2 = y + height;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            int unclippedy1 = y1, unclippedy2 = y2;
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;

            // Draw line
            if (color != null) {
                for (int iy = y1; iy < y2; iy++) {
                    ConsoleChar[] charsLine = GetLine(iy);
                    charsLine[x].ForegroundColor = color.Value;
                }
            }
            for (int iy = y1 + 1; iy < y2 - 1; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                LineChar lineChar = charsLine[x].LineChar;
                lineChar.SetVertical(lineWidth);
                charsLine[x].LineChar = lineChar;
            }

            // Cap start
            ConsoleChar[] charsLineStart = GetLine(y1);
            LineChar lineCharStart = charsLineStart[x].LineChar;
            if (y1 != unclippedy1)
                lineCharStart.SetVertical(lineWidth);
            else if (flags.HasCapStartFull())
                lineCharStart.SetVertical(lineWidth);
            else if (flags.HasCapStartCenter())
                lineCharStart.Bottom = lineWidth;
            charsLineStart[x].LineChar = lineCharStart;

            // Cap end
            ConsoleChar[] charsLineEnd = GetLine(y2 - 1);
            LineChar lineCharEnd = charsLineEnd[x].LineChar;
            if (y2 != unclippedy2)
                lineCharEnd.SetVertical(lineWidth);
            else if (flags.HasCapEndFull())
                lineCharEnd.SetVertical(lineWidth);
            else if (flags.HasCapEndCenter())
                lineCharEnd.Top = lineWidth;
            charsLineEnd[x].LineChar = lineCharEnd;
        }

        public void DrawLine(Line line, ConsoleColor? color = null, LineWidth lineWidth = LineWidth.Single,
            DrawLineFlags flags = DrawLineFlags.CapCenter)
        {
            if (line.IsHorizontal)
                DrawHorizontalLine(line.X, line.Y, line.Width, color, lineWidth, flags);
            else if (line.IsVertical)
                DrawVerticalLine(line.X, line.Y, line.Height, color, lineWidth, flags);
        }

        public void DrawRectangle(int x, int y, int width, int height, ConsoleColor? color = null, LineThickness? thickness = null)
        {
            if (thickness == null)
                thickness = LineThickness.Single;
            DrawHorizontalLine(x, y, width, color, thickness.Value.Top);
            DrawHorizontalLine(x, y + height - 1, width, color, thickness.Value.Bottom);
            DrawVerticalLine(x, y, height, color, thickness.Value.Left);
            DrawVerticalLine(x + width - 1, y, height, color, thickness.Value.Right);
        }

        public void DrawRectangle(Rect rect, ConsoleColor? color = null, LineThickness? thickness = null)
        {
            DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, thickness);
        }

        public void DrawRectangle(int x, int y, int width, int height, ConsoleColor? color = null, LineWidth lineWidth = LineWidth.Single)
        {
            DrawRectangle(x, y, width, height, color, new LineThickness(lineWidth));
        }

        public void DrawRectangle(Rect rect, ConsoleColor? color = null, LineWidth lineWidth = LineWidth.Single)
        {
            DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, lineWidth);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "IsNullOrEmpty validates the value.")]
        public void DrawString(int x, int y, ConsoleColor? color, string text)
        {
            if (text.IsNullOrEmpty())
                return;
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + text.Length;
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++) {
                if (color != null)
                    charsLine[ix].ForegroundColor = color.Value;
                charsLine[ix].Char = text[ix - x];
            }
        }

        public void FillForegroundHorizontalLine(int x, int y, int width, ConsoleColor? color = null, [ValueProvider(ValueProviders.Chars)] char fill = '\0')
        {
            int x1 = x, x2 = x + width;
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

        public void FillForegroundVerticalLine(int x, int y, int height, ConsoleColor? color = null, [ValueProvider(ValueProviders.Chars)] char fill = '\0')
        {
            int y1 = y, y2 = y + height;
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

        public void FillForegroundLine(Line line, ConsoleColor? color = null, [ValueProvider(ValueProviders.Chars)] char fill = '\0')
        {
            if (line.IsHorizontal)
                FillForegroundHorizontalLine(line.X, line.Y, line.Width, color, fill);
            else if (line.IsVertical)
                FillForegroundVerticalLine(line.X, line.Y, line.Height, color, fill);
        }

        public void FillForegroundRectangle(int x, int y, int width, int height, ConsoleColor? color = null, [ValueProvider(ValueProviders.Chars)] char fill = '\0')
        {
            int y1 = y, y2 = y + height;
            for (int iy = y1; iy < y2; iy++)
                FillForegroundHorizontalLine(x, iy, width, color, fill);
        }

        public void FillForegroundRectangle(Rect rect, ConsoleColor? color = null, [ValueProvider(ValueProviders.Chars)] char fill = '\0')
        {
            FillForegroundRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, fill);
        }

        public void FillBackgroundHorizontalLine(int x, int y, int width, ConsoleColor color)
        {
            int x1 = x, x2 = x + width;
            OffsetY(ref y).OffsetX(ref x1).OffsetX(ref x2);
            if (!ClipHorizontalLine(y, ref x1, ref x2))
                return;
            ConsoleChar[] charsLine = GetLine(y);
            for (int ix = x1; ix < x2; ix++)
                charsLine[ix].BackgroundColor = color;
        }

        public void FillBackgroundVerticalLine(int x, int y, int height, ConsoleColor color)
        {
            int y1 = y, y2 = y + height;
            OffsetX(ref x).OffsetY(ref y1).OffsetY(ref y2);
            if (!ClipVerticalLine(x, ref y1, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                charsLine[x].BackgroundColor = color;
            }
        }

        public void FillBackgroundLine(Line line, ConsoleColor color)
        {
            if (line.IsHorizontal)
                FillBackgroundHorizontalLine(line.X, line.Y, line.Width, color);
            else if (line.IsVertical)
                FillBackgroundVerticalLine(line.X, line.Y, line.Height, color);
        }

        public void FillBackgroundRectangle(int x, int y, int width, int height, ConsoleColor color)
        {
            int y1 = y, y2 = y + height;
            for (int iy = y1; iy < y2; iy++)
                FillBackgroundHorizontalLine(x, iy, width, color);
        }

        public void FillBackgroundRectangle(Rect rect, ConsoleColor color)
        {
            FillBackgroundRectangle(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public void ResetClip()
        {
            Clip = new Rect(0, 0, Width, Size.Infinity);
        }

        public void ApplyForegroundColorMap(int x, int y, int width, int height, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap)
        {
            ApplyColorMap(x, y, width, height, colorMap,
                (ref ConsoleChar c) => c.ForegroundColor = colorMap[(int)c.ForegroundColor]);
        }

        public void ApplyForegroundColorMap(Rect rect, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap)
        {
            ApplyForegroundColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap);
        }

        public void ApplyBackgroundColorMap(int x, int y, int width, int height, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap)
        {
            ApplyColorMap(x, y, width, height, colorMap,
                (ref ConsoleChar c) => c.BackgroundColor = colorMap[(int)c.BackgroundColor]);
        }

        public void ApplyBackgroundColorMap(Rect rect, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap)
        {
            ApplyBackgroundColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap);
        }

        public void ApplyColorMap(int x, int y, int width, int height, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap,
            [NotNull] ApplyColorMapCallback processChar)
        {
            if (colorMap == null)
                throw new ArgumentNullException(nameof(colorMap));
            if (processChar == null)
                throw new ArgumentNullException(nameof(processChar));
            if (colorMap.Count != ColorMaps.ConsoleColorCount)
                throw new ArgumentException("colorMap must contain 16 elements corresponding to each ConsoleColor.", nameof(colorMap));
            OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + width, y1 = y, y2 = y + height;
            if (!ClipRectangle(ref x1, ref y1, ref x2, ref y2))
                return;
            for (int iy = y1; iy < y2; iy++) {
                ConsoleChar[] charsLine = GetLine(iy);
                for (int ix = x1; ix < x2; ix++)
                    processChar(ref charsLine[ix]);
            }
        }

        public void ApplyColorMap(Rect rect, [ValueProvider(ValueProviders.ColorMaps), NotNull] IList<ConsoleColor> colorMap,
            [NotNull] ApplyColorMapCallback processChar)
        {
            ApplyColorMap(rect.X, rect.Y, rect.Width, rect.Width, colorMap, processChar);
        }

        internal bool ClipHorizontalLine(int y, ref int x1, ref int x2)
        {
            if (!Clip.IntersectsHorizontalLine(y))
                return false;
            x1 = Math.Max(x1, Clip.X);
            x2 = Math.Min(x2, Clip.Right);
            return x1 != x2;
        }

        internal bool ClipVerticalLine(int x, ref int y1, ref int y2)
        {
            if (!Clip.IntersectsVerticalLine(x))
                return false;
            y1 = Math.Max(y1, Clip.Y);
            y2 = Math.Min(y2, Clip.Bottom);
            return y1 != y2;
        }

        internal bool ClipRectangle(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            x1 = Math.Max(x1, Clip.X);
            x2 = Math.Min(x2, Clip.Right);
            y1 = Math.Max(y1, Clip.Y);
            y2 = Math.Min(y2, Clip.Bottom);
            return x1 < x2 && y1 < y2;
        }

        internal ConsoleBuffer OffsetX(ref int x)
        {
            x += Offset.X;
            return this;
        }

        internal ConsoleBuffer OffsetY(ref int y)
        {
            y += Offset.Y;
            return this;
        }

        internal ConsoleChar[] GetLine(int y)
        {
            while (_chars.Count <= y)
                _chars.Add(new ConsoleChar[Width]);
            if (Height <= y)
                Height = y + 1;
            return _chars[y];
        }

        ConsoleChar[] IConsoleBufferSource.GetLine(int y)
        {
            return GetLine(y);
        }

        private ConsoleChar? GetChar(int x, int y)
        {
            if (!new Rect(0, 0, Width, Height).Contains(x, y))
                return null;
            return GetLine(y)[x];
        }

        ConsoleChar? IConsoleBufferSource.GetChar(int x, int y)
        {
            return GetChar(x, y);
        }

        char IConsoleBufferSource.GetLineChar(int x, int y)
        {
            ConsoleChar? chr = GetChar(x, y);
            return chr == null ? '\0' : LineCharRenderer.GetChar(chr.Value.LineChar);
        }
    }
}