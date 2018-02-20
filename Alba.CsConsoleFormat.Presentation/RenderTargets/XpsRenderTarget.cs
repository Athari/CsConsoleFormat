using System;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public sealed class XpsRenderTarget : DocumentRenderTargetBase
    {
        private readonly Stream _output;
        private readonly bool _leaveOpen;

        public PresentationDocumentType DocumentType { get; set; } = PresentationDocumentType.FlowDocument;

        public XpsRenderTarget([NotNull] Stream output, bool leaveOpen = false)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _leaveOpen = leaveOpen;
        }

        public override void Render(IConsoleBufferSource buffer)
        {
            IDocumentPaginatorSource document;
            if (DocumentType == PresentationDocumentType.FlowDocument) {
                document = new FlowDocument();
                RenderToFlowDocument(buffer, (FlowDocument)document);
            }
            else {
                document = new FixedDocument();
                RenderToFixedDocument(buffer, (FixedDocument)document);
            }
            SaveDocument(document);
        }

        private void SaveDocument([NotNull] IDocumentPaginatorSource document)
        {
            using (var package = Package.Open(_output, FileMode.Create, FileAccess.ReadWrite))
            using (var xps = new XpsDocument(package, CompressionOption.Maximum))
            using (var policy = new XpsPackagingPolicy(xps))
            using (var serializer = new XpsSerializationManager(policy, false)) {
                //document.DocumentPaginator.PageSize = new System.Windows.Size(100, 100);
                serializer.SaveAsXaml(document.DocumentPaginator);
                serializer.Commit();
            }
            if (_leaveOpen)
                _output.Flush();
            else
                _output.Dispose();
        }
    }
}