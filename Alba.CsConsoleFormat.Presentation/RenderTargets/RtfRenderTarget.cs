using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public sealed class RtfRenderTarget : DocumentRenderTargetBase
    {
        private readonly Stream _output;
        private readonly bool _leaveOpen;

        public RtfRenderTarget([NotNull] Stream output, bool leaveOpen = false)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _leaveOpen = leaveOpen;
        }

        public override void Render(IConsoleBufferSource buffer)
        {
            var document = new FlowDocument();
            RenderToFlowDocument(buffer, document);
            var content = new TextRange(document.ContentStart, document.ContentEnd);
            content.Save(_output, DataFormats.Rtf);
            if (_leaveOpen)
                _output.Flush();
            else
                _output.Dispose();
        }
    }
}