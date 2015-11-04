using System.IO;
using Alba.CsConsoleFormat.Presentation;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.Presentation
{
    public partial class MainWindow
    {
        public Document Document (string world) => CreateDocument(world);
        public Document FixedDocument => Document("fixed");
        public Document FlowDocument => Document("flow");

        public MainWindow ()
        {
            InitializeComponent();

            var renderRect = new Rect(0, 0, 20, Size.Infinity);

            ConsoleRenderer.RenderDocument(
                CreateDocument("fixed XPS"),
                new XpsRenderTarget(File.Create(@"../../Tmp/0a.xps")) { FontSize = 16, DocumentType = PresentationDocumentType.FixedDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                CreateDocument("flow XPS"),
                new XpsRenderTarget(File.Create(@"../../Tmp/0b.xps")) { FontSize = 16, DocumentType = PresentationDocumentType.FlowDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                CreateDocument("RTF"),
                new RtfRenderTarget(File.Create(@"../../Tmp/0.rtf")) { FontSize = 16 },
                renderRect);
        }

        public Document CreateDocument (string world) =>
            new Document { Color = White, BgColor = Black }
                .AddChildren(
                    new Div { Color = Red }.AddChildren($"Hello {world} world!"),
                    new Div { Color = Red }.AddChildren("Hello world!"),
                    new Div()
                        .AddChildren(
                            new Span("Foo") { Color = Yellow, BgColor = DarkYellow },
                            " ",
                            new Span("Bar") { Color = Cyan, BgColor = DarkCyan }
                        )
                );
    }
}