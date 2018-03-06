using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class TextRenderTarget : TextRenderTargetBase
    {
        public TextRenderTarget([NotNull] Stream output, [CanBeNull] Encoding encoding = null, bool leaveOpen = false) : base(output, encoding, leaveOpen)
        { }

        public TextRenderTarget([CanBeNull] TextWriter writer = null) : base(writer)
        { }

        protected override void RenderOverride(IConsoleBufferSource buffer)
        {
            ThrowIfDisposed();
            Debug.Assert(Writer != null);
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);
                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleChar chr = charsLine[ix];
                    Writer.Write(chr.HasChar || chr.LineChar.IsEmpty ? chr.PrintableChar : buffer.GetLineChar(ix, iy));
                }
                Writer.WriteLine();
            }
        }
    }
}