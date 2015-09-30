using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using static Alba.CsConsoleFormat.Presentation.FontDefaults;

namespace Alba.CsConsoleFormat.Presentation.Markup
{
    [ValueConversion (typeof(Document), typeof(FixedDocument))]
    [ValueConversion (typeof(Document), typeof(FlowDocument))]
    public class DocumentConverter : IValueConverter
    {
        public PresentationDocumentType DocumentType { get; set; } = PresentationDocumentType.FixedDocument;

        public Brush Background { get; set; } = DefaultConsoleBackground;
        public int ConsoleWidth { get; set; } = DefaultConsoleWidth;

        public FontFamily FontFamily { get; set; } = DefaultFontFamily;
        public double FontSize { get; set; } = DefaultFontSize;
        public FontStretch FontStretch { get; set; } = DefaultFontStretch;
        public FontStyle FontStyle { get; set; } = DefaultFontStyle;
        public FontWeight FontWeight { get; set; } = DefaultFontWeight;

        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var document = value as Document;
            if (document == null)
                return null;

            FrameworkContentElement content;
            DocumentRenderTargetBase target;
            if (DocumentType == PresentationDocumentType.FixedDocument) {
                var fixedDocument = new FixedDocument();
                target = new FixedDocumentRenderTarget(fixedDocument);
                content = fixedDocument;
            }
            else {
                var flowDocument = new FlowDocument();
                target = new FlowDocumentRenderTarget(flowDocument);
                content = flowDocument;
            }

            target.Background = Background;
            target.FontFamily = FontFamily;
            target.FontSize = FontSize;
            target.FontStretch = FontStretch;
            target.FontStyle = FontStyle;
            target.FontWeight = FontWeight;

            ConsoleRenderer.RenderDocument(document, target, new Rect(new Size(ConsoleWidth, Size.Infinity)));

            return content;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}