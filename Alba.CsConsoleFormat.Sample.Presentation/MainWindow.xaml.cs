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

            var fixedDoc = new FixedDocument();
            var renderRect = new Rect(0, 0, 20, Size.Infinity);
            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new FixedDocumentRenderTarget(fixedDoc) {
                    FontSize = 16,
                },
                renderRect);
            dvwFixed.Document = fixedDoc;

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument(),
                new XpsDocumentRenderTarget(@"../../Tmp/0.xps") {
                    FontSize = 16,
                },
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
                    Create<Div>()
                        .AddChildren(
                            CreateText("Foo").Color(Yellow, DarkYellow),
                            " ",
                            CreateText("Bar").Color(Cyan, DarkCyan)
                        )
                );
    }
}