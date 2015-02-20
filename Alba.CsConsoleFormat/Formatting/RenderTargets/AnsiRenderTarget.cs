using System;
using System.Collections.Generic;
using System.Text;

namespace Alba.CsConsoleFormat
{
    public class AnsiRenderTarget : IRenderTarget
    {
        private static readonly List<string> ColorMap = new List<string> {
            "0;30", "0;34", "0;32", "0;36", "0;31", "0;35", "0;33", "0;37",
            "1;30", "1;34", "1;32", "1;36", "1;31", "1;35", "1;33", "1;37",
            //"90", "94", "92", "96", "91", "95", "93", "97",
        };
        private static readonly List<string> BgColorMap = new List<string> {
            //"40", "44", "42", "46", "41", "45", "43", "47",
            "25;40", "25;44", "25;42", "25;46", "25;41", "25;45", "25;43", "25;47",
            "5;40", "5;44", "5;42", "5;46", "5;41", "5;45", "5;43", "5;47",
            //"100", "104", "102", "106", "101", "105", "103", "107",
        };

        private readonly StringBuilder _text = new StringBuilder();

        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BgColorOverride { get; set; }
        public string NewLine { get; set; }

        public AnsiRenderTarget ()
        {
            ColorOverride = null;
            BgColorOverride = null;
            NewLine = "";
        }

        public string OutputText
        {
            get { return _text.ToString(); }
        }

        public void Render (IConsoleBufferSource buffer)
        {
            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;

            _text.Clear();
            _text.Append("\x1B[0m");

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleColor foreColor = ColorOverride ?? charsLine[ix].ForegroundColor;
                    ConsoleColor backColor = BgColorOverride ?? charsLine[ix].BackgroundColor;
                    if (foreColor != currentForeColor || backColor != currentBackColor) {
                        _text.Append("\x1B[")
                            .Append(ColorMap[(int)foreColor]).Append(";")
                            .Append(BgColorMap[(int)backColor]).Append("m");
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    LineChar lineChr = charsLine[ix].LineChar;
                    char chr = charsLine[ix].Char;
                    if (!lineChr.IsEmpty() && chr == '\0')
                        chr = buffer.GetLineChar(ix, iy);
                    _text.Append(buffer.SafeChar(chr));
                }
                _text.Append(NewLine);
            }
        }
    }
}