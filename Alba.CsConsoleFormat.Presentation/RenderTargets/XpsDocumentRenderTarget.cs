using System;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public class XpsDocumentRenderTarget : DocumentRenderTargetBase
    {
        private readonly Stream _output;
        private readonly bool _leaveOpen;

        public XpsDocumentType DocumentType { get; set; } = XpsDocumentType.FlowDocument;

        public XpsDocumentRenderTarget ([NotNull] Stream output, bool leaveOpen = false)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            _output = output;
            _leaveOpen = leaveOpen;
        }

        public override void Render (IConsoleBufferSource buffer)
        {
            IDocumentPaginatorSource document;
            if (DocumentType == XpsDocumentType.FlowDocument) {
                document = new FlowDocument();
                RenderToFlowDocument(buffer, (FlowDocument)document);
            }
            else {
                document = new FixedDocument();
                RenderToFixedDocument(buffer, (FixedDocument)document);
            }
            SaveDocument(document);
        }

        private void SaveDocument (IDocumentPaginatorSource document)
        {
            using (Package package = Package.Open(_output, FileMode.Create, FileAccess.ReadWrite))
            using (XpsDocument xps = new XpsDocument(package, CompressionOption.Maximum)) {
                var serializer = new XpsSerializationManager(new XpsPackagingPolicy(xps), false);
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