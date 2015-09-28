using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Alba.CsConsoleFormat.Presentation
{
    public static class ConsoleBufferImageExts
    {
        private static readonly Dictionary<Color, ConsoleColor> ConsolePaletteMap = new Dictionary<Color, ConsoleColor> {
            [Color.FromRgb(0, 0, 0)] = ConsoleColor.Black,
            [Color.FromRgb(0, 0, 128)] = ConsoleColor.DarkBlue,
            [Color.FromRgb(0, 128, 0)] = ConsoleColor.DarkGreen,
            [Color.FromRgb(0, 128, 128)] = ConsoleColor.DarkCyan,
            [Color.FromRgb(128, 0, 0)] = ConsoleColor.DarkRed,
            [Color.FromRgb(128, 0, 128)] = ConsoleColor.DarkMagenta,
            [Color.FromRgb(128, 128, 0)] = ConsoleColor.DarkYellow,
            [Color.FromRgb(192, 192, 192)] = ConsoleColor.Gray,
            [Color.FromRgb(219, 219, 219)] = ConsoleColor.Gray,
            [Color.FromRgb(128, 128, 128)] = ConsoleColor.DarkGray,
            [Color.FromRgb(0, 0, 255)] = ConsoleColor.Blue,
            [Color.FromRgb(0, 255, 0)] = ConsoleColor.Green,
            [Color.FromRgb(0, 255, 255)] = ConsoleColor.Cyan,
            [Color.FromRgb(255, 0, 0)] = ConsoleColor.Red,
            [Color.FromRgb(255, 0, 255)] = ConsoleColor.Magenta,
            [Color.FromRgb(255, 255, 0)] = ConsoleColor.Yellow,
            [Color.FromRgb(255, 255, 255)] = ConsoleColor.White,
        };

        public static void DrawImage (this ConsoleBuffer @this, ImageSource imageSource, int x, int y, int width, int height)
        {
            var bmp = imageSource as BitmapSource;
            if (bmp == null)
                throw new ArgumentException("Only rendering of bitmap source is supported.");

            @this.OffsetX(ref x).OffsetY(ref y);
            int x1 = x, x2 = x + width, y1 = y, y2 = y + height;
            if (!@this.ClipRectangle(ref x1, ref y1, ref x2, ref y2))
                return;

            if (width != bmp.PixelWidth || height != bmp.PixelHeight)
                bmp = new TransformedBitmap(bmp, new ScaleTransform((double)width / bmp.PixelWidth, (double)height / bmp.PixelHeight));
            if (bmp.Format != PixelFormats.Indexed4)
                bmp = new FormatConvertedBitmap(bmp, PixelFormats.Indexed4, BitmapPalettes.Halftone8Transparent, 0.5);

            const int bitsPerPixel = 4;
            int stride = 4 * (bmp.PixelWidth * bitsPerPixel + 31) / 32;
            byte[] bytes = new byte[stride * bmp.PixelHeight];
            bmp.CopyPixels(bytes, stride, 0);

            for (int iy = y1, py = 0; iy < y2; iy++, py++) {
                ConsoleChar[] charsLine = @this.GetLine(iy);
                for (int ix = x1, px = 0; ix < x2; ix++, px++) {
                    int byteIndex = stride * py + px / 2;
                    int bitOffset = px % 2 == 0 ? 4 : 0;
                    SetColor(ref charsLine[ix], bmp.Palette, (bytes[byteIndex] >> bitOffset) & 0xF);
                }
            }
        }

        private static void SetColor (ref ConsoleChar c, BitmapPalette palette, int colorIndex)
        {
            ConsoleColor color;
            if (ConsolePaletteMap.TryGetValue(palette.Colors[colorIndex], out color))
                c.BackgroundColor = color;
        }

        public static void DrawImage (this ConsoleBuffer @this, ImageSource imageSource, Rect rect)
        {
            @this.DrawImage(imageSource, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}