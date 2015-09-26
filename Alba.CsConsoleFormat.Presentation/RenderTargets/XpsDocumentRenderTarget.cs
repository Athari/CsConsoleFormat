using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Presentation
{
    public class XpsDocumentRenderTarget : FixedDocumentRenderTargetBase
    {
        private readonly string _fileName;

        public XpsDocumentRenderTarget ([NotNull] string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            _fileName = fileName;
        }

        public override void Render (IConsoleBufferSource buffer)
        {
            var document = new FixedDocument();
            RenderToFixedDocument(buffer, document);
            File.Delete(_fileName);
            using (XpsDocument xps = new XpsDocument(_fileName, FileAccess.ReadWrite))
                XpsDocument.CreateXpsDocumentWriter(xps).Write(document);
        }
    }
}