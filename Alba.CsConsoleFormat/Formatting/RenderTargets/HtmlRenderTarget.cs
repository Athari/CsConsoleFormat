using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    public class HtmlRenderTarget : IRenderTarget
    {
        private readonly StringBuilder _html = new StringBuilder();

        private static readonly List<string> ColorMap = new List<string> {
            "000", "008", "080", "088", "800", "808", "880", "CCC",
            "888", "00F", "0F0", "0FF", "F00", "F0F", "FF0", "FFF",
        };

        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BgColorOverride { get; set; }

        public bool BodyOnly { get; set; }
        public string Charset { get; set; }
        public string Font { get; set; }
        public string PageTitle { get; set; }

        public HtmlRenderTarget ()
        {
            ColorOverride = null;
            BgColorOverride = null;

            Charset = "utf-8";
            BodyOnly = false;
            Font = "12pt/100% \"Lucida Console\", Consolas, monospace";
            PageTitle = "Console output";
        }

        public string OutputHtml
        {
            get { return _html.ToString(); }
        }

        public void Render (IConsoleBufferSource buffer)
        {
            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;

            _html.Clear();
            if (!BodyOnly) {
                _html.Append("<!DOCTYPE html>\n<html>\n<head>\n  <meta charset=\"")
                    .Append(Charset)
                    .Append("\">\n  <title>")
                    .Append(WebUtility.HtmlEncode(PageTitle))
                    .Append("</title>\n  <style>\n    pre { font: ")
                    .Append(Font)
                    .Append(" }\n  </style>\n</head>\n<body>\n");
            }

            _html.Append("<pre><span>");

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleColor foreColor = ColorOverride ?? charsLine[ix].ForegroundColor;
                    ConsoleColor backColor = BgColorOverride ?? charsLine[ix].BackgroundColor;
                    if (foreColor != currentForeColor || backColor != currentBackColor) {
                        _html.Append("</span><span style=\"color:#{0};background:#{1}\">"
                            .FmtInv(ColorMap[(int)foreColor], ColorMap[(int)backColor]));
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    LineChar lineChr = charsLine[ix].LineChar;
                    char chr = charsLine[ix].Char;
                    if (!lineChr.IsEmpty() && chr == '\0')
                        chr = buffer.GetLineChar(ix, iy);
                    _html.Append(WebUtility.HtmlEncode(buffer.SafeChar(chr).ToString()));
                }

                _html.Append("<br/>");
            }

            _html.Append("</pre>");

            if (!BodyOnly)
                _html.Append("\n</body>\n</html>");
        }
    }
}