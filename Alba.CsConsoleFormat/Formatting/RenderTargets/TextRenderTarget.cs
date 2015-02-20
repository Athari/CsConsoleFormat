using System;
using System.Text;

namespace Alba.CsConsoleFormat
{
    public class TextRenderTarget
    {
        private readonly StringBuilder _text = new StringBuilder();

        public string NewLine { get; set; }

        public TextRenderTarget ()
        {
            NewLine = Environment.NewLine;
        }

        public string OutputText
        {
            get { return _text.ToString(); }
        }

        public void Render (IConsoleBufferSource buffer)
        {
            _text.Clear();

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);

                for (int ix = 0; ix < buffer.Width; ix++) {
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