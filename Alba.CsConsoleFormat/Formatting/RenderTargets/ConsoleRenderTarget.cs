using System;
using System.IO;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderTarget : IRenderTarget
    {
        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BackgroundOverride { get; set; }

        public ConsoleRenderTarget ()
        {
            ColorOverride = null;
            BackgroundOverride = null;
        }

        public void Render (IConsoleBufferSource buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            ConsoleColor currentForeColor = Console.ForegroundColor, oldForeColor = currentForeColor;
            ConsoleColor currentBackColor = Console.BackgroundColor, oldBackColor = currentBackColor;
            bool isManualWrap;
            try {
                isManualWrap = buffer.Width < Console.BufferWidth;
            }
            catch (IOException) {
                isManualWrap = true;
            }
            try {
                for (int iy = 0; iy < buffer.Height; iy++) {
                    ConsoleChar[] charsLine = buffer.GetLine(iy);

                    for (int ix = 0; ix < buffer.Width; ix++) {
                        ConsoleChar chr = charsLine[ix];
                        ConsoleColor foreColor = ColorOverride ?? chr.ForegroundColor;
                        if (foreColor != currentForeColor)
                            Console.ForegroundColor = currentForeColor = foreColor;
                        ConsoleColor backColor = BackgroundOverride ?? chr.BackgroundColor;
                        if (backColor != currentBackColor)
                            Console.BackgroundColor = currentBackColor = backColor;
                        Console.Write(chr.HasChar || chr.LineChar.IsEmpty() ? chr.PrintableChar : buffer.GetLineChar(ix, iy));
                    }
                    if (isManualWrap)
                        Console.WriteLine();
                }
            }
            finally {
                Console.ForegroundColor = oldForeColor;
                Console.BackgroundColor = oldBackColor;
            }
        }
    }
}