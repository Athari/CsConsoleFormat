using System;
using System.Windows.Documents;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public class FlowDocumentRenderTarget : DocumentRenderTargetBase
    {
        public FlowDocument Document { get; }

        public FlowDocumentRenderTarget ([NotNull] FlowDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            Document = document;
        }

        public override void Render (IConsoleBufferSource buffer)
        {
            RenderToFlowDocument(buffer, Document);
        }
    }
}