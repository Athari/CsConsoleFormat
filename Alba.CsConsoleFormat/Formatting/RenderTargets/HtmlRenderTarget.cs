using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Alba.CsConsoleFormat
{
    public class HtmlRenderTarget : TextRenderTargetBase
    {
        private static readonly List<string> ColorMap = new List<string> {
            "000", "008", "080", "088", "800", "808", "880", "CCC",
            "888", "00F", "0F0", "0FF", "F00", "F0F", "FF0", "FFF",
        };

        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BgColorOverride { get; set; }
        public bool BodyOnly { get; set; }
        public string Font { get; set; }
        public string PageTitle { get; set; }

        public HtmlRenderTarget (Stream output, Encoding encoding = null, bool leaveOpen = false) : base(output, encoding, leaveOpen)
        {
            Init();
        }

        public HtmlRenderTarget (TextWriter writer = null) : base(writer)
        {
            Init();
        }

        private void Init ()
        {
            ColorOverride = null;
            BgColorOverride = null;
            BodyOnly = false;
            Font = "12pt/100% \"Lucida Console\", Consolas, monospace";
            PageTitle = "Console output";
        }

        protected override void RenderOverride (IConsoleBufferSource buffer)
        {
            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;

            if (!BodyOnly) {
                Writer.Write("<!DOCTYPE html>\n<html>\n<head>\n  <meta charset=\"");
                Writer.Write(Writer.Encoding.WebName);
                Writer.Write("\">\n  <title>");
                Writer.Write(WebUtility.HtmlEncode(PageTitle));
                Writer.Write("</title>\n  <style>\n    pre { font: ");
                Writer.Write(Font);
                Writer.Write(" }\n  </style>\n</head>\n<body>\n");
            }

            Writer.Write("<pre><span>");

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleColor foreColor = ColorOverride ?? charsLine[ix].ForegroundColor;
                    ConsoleColor backColor = BgColorOverride ?? charsLine[ix].BackgroundColor;
                    if (foreColor != currentForeColor || backColor != currentBackColor) {
                        Writer.Write($"</span><span style=\"color:#{ColorMap[(int)foreColor]};background:#{ColorMap[(int)backColor]}\">");
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    char chr = charsLine[ix].Char;
                    LineChar lineChr = charsLine[ix].LineChar;
                    if (!lineChr.IsEmpty() && chr == '\0')
                        chr = buffer.GetLineChar(ix, iy);
                    Writer.Write(WebUtility.HtmlEncode(buffer.SafeChar(chr).ToString()));
                }

                Writer.Write("<br/>");
            }

            Writer.Write("</pre>");
            if (!BodyOnly)
                Writer.Write("\n</body>\n</html>");
        }
    }
}