using System;
using System.Windows.Documents;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public sealed class FlowDocumentRenderTarget : DocumentRenderTargetBase
    {
        public FlowDocument Document { get; }

        public FlowDocumentRenderTarget([NotNull] FlowDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public override void Render(IConsoleBufferSource buffer)
        {
            RenderToFlowDocument(buffer, Document);
        }
    }
}