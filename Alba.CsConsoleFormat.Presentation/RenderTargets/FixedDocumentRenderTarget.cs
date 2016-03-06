using System;
using System.Windows.Documents;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public class FixedDocumentRenderTarget : DocumentRenderTargetBase
    {
        public FixedDocument Document { get; }

        public FixedDocumentRenderTarget([NotNull] FixedDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            Document = document;
        }

        public override void Render(IConsoleBufferSource buffer)
        {
            RenderToFixedDocument(buffer, Document);
        }
    }
}