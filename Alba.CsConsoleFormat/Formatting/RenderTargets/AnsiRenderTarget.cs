using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class AnsiRenderTarget : TextRenderTargetBase
    {
        private static readonly List<string> ColorMap = new List<string> {
            "30", "34", "32", "36", "31", "35", "33", "37",
            //"0;30", "0;34", "0;32", "0;36", "0;31", "0;35", "0;33", "0;37",
            //"1;30", "1;34", "1;32", "1;36", "1;31", "1;35", "1;33", "1;37",
            "90", "94", "92", "96", "91", "95", "93", "97",
        };
        private static readonly List<string> BgColorMap = new List<string> {
            "40", "44", "42", "46", "41", "45", "43", "47",
            //"25;40", "25;44", "25;42", "25;46", "25;41", "25;45", "25;43", "25;47",
            //"5;40", "5;44", "5;42", "5;46", "5;41", "5;45", "5;43", "5;47",
            "100", "104", "102", "106", "101", "105", "103", "107",
        };

        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BgColorOverride { get; set; }

        public AnsiRenderTarget ([NotNull] Stream output, Encoding encoding = null, bool leaveOpen = false) : base(output, encoding, leaveOpen)
        {}

        public AnsiRenderTarget (TextWriter writer = null) : base(writer)
        {}

        protected override void RenderOverride (IConsoleBufferSource buffer)
        {
            ThrowIfDisposed();
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;

            Writer.Write("\x1B[0m");

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleColor foreColor = ColorOverride ?? charsLine[ix].ForegroundColor;
                    ConsoleColor backColor = BgColorOverride ?? charsLine[ix].BackgroundColor;
                    if (foreColor != currentForeColor || backColor != currentBackColor) {
                        Writer.Write("\x1B[");
                        Writer.Write(ColorMap[(int)foreColor]);
                        Writer.Write(";");
                        Writer.Write(BgColorMap[(int)backColor]);
                        Writer.Write("m");
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    char chr = charsLine[ix].Char;
                    LineChar lineChr = charsLine[ix].LineChar;
                    if (!lineChr.IsEmpty() && chr == '\0')
                        chr = buffer.GetLineChar(ix, iy);
                    Writer.Write(buffer.SafeChar(chr));
                }
                Writer.WriteLine();
            }
        }
    }
}