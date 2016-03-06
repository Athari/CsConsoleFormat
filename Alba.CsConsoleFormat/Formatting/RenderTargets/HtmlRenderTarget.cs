using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class HtmlRenderTarget : TextRenderTargetBase
    {
        private static readonly List<string> ColorMap = new List<string> {
            "000", "008", "080", "088", "800", "808", "880", "CCC",
            "888", "00F", "0F0", "0FF", "F00", "F0F", "FF0", "FFF",
        };

        public ConsoleColor? ColorOverride { get; set; }
        public ConsoleColor? BackgroundOverride { get; set; }
        public bool BodyOnly { get; set; }
        public string Font { get; set; }
        public string PageTitle { get; set; }

        public HtmlRenderTarget([NotNull] Stream output, Encoding encoding = null, bool leaveOpen = false) : base(output, encoding, leaveOpen)
        {
            Init();
        }

        public HtmlRenderTarget(TextWriter writer = null) : base(writer)
        {
            Init();
        }

        private void Init()
        {
            ColorOverride = null;
            BackgroundOverride = null;
            BodyOnly = false;
            Font = "12pt/100% Consolas, \"Lucida Console\", monospace";
            PageTitle = "Console output";
        }

        protected override void RenderOverride(IConsoleBufferSource buffer)
        {
            ThrowIfDisposed();
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;

            if (!BodyOnly) {
                Writer.Write($@"<!DOCTYPE html>
                    <html>
                    <head>
                      <meta charset=""{Writer.Encoding.WebName}"">
                      <title>{WebUtility.HtmlEncode(PageTitle)}</title>
                      <style>
                        pre {{ font: {Font} }}
                      </style>
                    </head>
                    <body>
                    ".Replace("\n                    ", "\n"));
            }

            Writer.Write("<pre><span>");

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleChar chr = charsLine[ix];
                    ConsoleColor foreColor = ColorOverride ?? chr.ForegroundColor;
                    ConsoleColor backColor = BackgroundOverride ?? chr.BackgroundColor;
                    if (foreColor != currentForeColor || backColor != currentBackColor) {
                        Writer.Write($"</span><span style=\"color:#{ColorMap[(int)foreColor]};background:#{ColorMap[(int)backColor]}\">");
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    char c = chr.HasChar || chr.LineChar.IsEmpty() ? chr.PrintableChar : buffer.GetLineChar(ix, iy);
                    Writer.Write(WebUtility.HtmlEncode(c.ToString()));
                }

                Writer.Write("<br/>");
            }

            Writer.Write("</pre>");
            if (!BodyOnly)
                Writer.Write("\n</body>\n</html>");
        }
    }
}