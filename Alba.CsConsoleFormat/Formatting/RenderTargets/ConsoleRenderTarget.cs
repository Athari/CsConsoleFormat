using System;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderTarget : IRenderTarget
    {
        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BgColorOverride { get; set; }

        public ConsoleRenderTarget ()
        {
            ColorOverride = null;
            BgColorOverride = null;
        }

        public void Render (IConsoleBufferSource buffer)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor, oldForeColor = currentForeColor;
            ConsoleColor currentBackColor = Console.BackgroundColor, oldBackColor = currentBackColor;
            bool isManualWrap = buffer.Width < Console.BufferWidth;
            try {
                for (int iy = 0; iy < buffer.Height; iy++) {
                    ConsoleChar[] charsLine = buffer.GetLine(iy);

                    for (int ix = 0; ix < buffer.Width; ix++) {
                        ConsoleColor foreColor = ColorOverride ?? charsLine[ix].ForegroundColor;
                        if (foreColor != currentForeColor)
                            Console.ForegroundColor = currentForeColor = foreColor;
                        ConsoleColor backColor = BgColorOverride ?? charsLine[ix].BackgroundColor;
                        if (backColor != currentBackColor)
                            Console.BackgroundColor = currentBackColor = backColor;
                        LineChar lineChr = charsLine[ix].LineChar;
                        char chr = charsLine[ix].Char;
                        if (!lineChr.IsEmpty() && chr == '\0')
                            chr = buffer.GetLineChar(ix, iy);
                        Console.Write(buffer.SafeChar(chr));
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