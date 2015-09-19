using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Alba.CsConsoleFormat
{
    public abstract class TextRenderTargetBase : IRenderTarget
    {
        private readonly bool _leaveOpen;

        protected TextWriter Writer { get; }

        protected TextRenderTargetBase (Stream output, Encoding encoding = null, bool leaveOpen = false)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            Writer = new StreamWriter(output, encoding ?? Encoding.Unicode);
            _leaveOpen = leaveOpen;
        }

        protected TextRenderTargetBase (TextWriter writer = null)
        {
            Writer = writer ?? new StringWriter(CultureInfo.InvariantCulture);
        }

        public string OutputText
        {
            get
            {
                var stringWriter = Writer as StringWriter;
                if (stringWriter == null)
                    throw new InvalidOperationException("Cannot get output text, output is not StringWriter.");
                return stringWriter.ToString();
            }
        }

        public void Render (IConsoleBufferSource buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            RenderOverride(buffer);
            if (_leaveOpen)
                Writer.Flush();
            else
                Writer.Dispose();
        }

        protected abstract void RenderOverride (IConsoleBufferSource buffer);
    }
}