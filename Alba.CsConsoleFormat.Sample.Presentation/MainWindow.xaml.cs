using System.IO;
using System.Windows.Documents;
using Alba.CsConsoleFormat.Generation;
using Alba.CsConsoleFormat.Presentation;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.Presentation
{
    public partial class MainWindow
    {
        public MainWindow ()
        {
            InitializeComponent();

            var renderRect = new Rect(0, 0, 20, Size.Infinity);

            var fixedDoc = new FixedDocument();
            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new FixedDocumentRenderTarget(fixedDoc) { FontSize = 16 },
                renderRect);
            dvwFixed.Document = fixedDoc;

            var flowDoc = new FlowDocument();
            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new FlowDocumentRenderTarget(flowDoc) { FontSize = 16 },
                renderRect);
            dvwFlow.Document = flowDoc;

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new XpsDocumentRenderTarget(File.Create(@"../../Tmp/0a.xps")) { FontSize = 16, DocumentType = XpsDocumentType.FixedDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new XpsDocumentRenderTarget(File.Create(@"../../Tmp/0b.xps")) { FontSize = 16, DocumentType = XpsDocumentType.FlowDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new RtfDocumentRenderTarget(File.Create(@"../../Tmp/0.rtf")) { FontSize = 16 },
                renderRect);

            cvwConsole.Document = new ViewBuilder().CreateDocument();
        }
    }

    public class ViewBuilder : DocumentBuilder
    {
        public Document CreateDocument () =>
            Create<Document>()
                .Color(White, Black)
                .AddChildren(
                    Create<Div>("Hello world!").Color(Red),
                    Create<Div>("Hello world!").Color(Red),
                    Create<Div>()
                        .AddChildren(
                            CreateText("Foo").Color(Yellow, DarkYellow),
                            " ",
                            CreateText("Bar").Color(Cyan, DarkCyan)
                        )
                );
    }
}