using System;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public abstract class TextRenderTargetBase : IRenderTarget, IDisposable
    {
        private static readonly Encoding DefaultEncoding = new UnicodeEncoding(false, false);

        private readonly bool _leaveOpen;

        protected TextWriter Writer { get; private set; }

        protected TextRenderTargetBase ([NotNull] Stream output, Encoding encoding = null, bool leaveOpen = false)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            Writer = new StreamWriter(output, encoding ?? DefaultEncoding);
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
                ThrowIfDisposed();
                var stringWriter = Writer as StringWriter;
                if (stringWriter == null)
                    throw new InvalidOperationException($"Cannot get output text, output is not {nameof(StringWriter)}.");
                return stringWriter.ToString();
            }
        }

        public void Render (IConsoleBufferSource buffer)
        {
            ThrowIfDisposed();
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            RenderOverride(buffer);
        }

        protected abstract void RenderOverride ([NotNull] IConsoleBufferSource buffer);

        protected void ThrowIfDisposed ()
        {
            if (Writer == null)
                throw new ObjectDisposedException(null);
        }

        public virtual void Dispose ()
        {
            if (Writer == null)
                return;
            if (_leaveOpen)
                Writer.Flush();
            else
                Writer.Dispose();
            Writer = null;
        }
    }
}