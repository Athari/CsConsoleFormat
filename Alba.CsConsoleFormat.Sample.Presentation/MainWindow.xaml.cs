using System.IO;
using Alba.CsConsoleFormat.Generation;
using Alba.CsConsoleFormat.Presentation;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.Presentation
{
    public partial class MainWindow
    {
        public Document Document (string world) => new ViewBuilder().CreateDocument(world);
        public Document FixedDocument => Document("fixed");
        public Document FlowDocument => Document("flow");

        public MainWindow ()
        {
            InitializeComponent();

            var renderRect = new Rect(0, 0, 20, Size.Infinity);

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument("fixed XPS"),
                new XpsRenderTarget(File.Create(@"../../Tmp/0a.xps")) { FontSize = 16, DocumentType = PresentationDocumentType.FixedDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument("flow XPS"),
                new XpsRenderTarget(File.Create(@"../../Tmp/0b.xps")) { FontSize = 16, DocumentType = PresentationDocumentType.FlowDocument },
                renderRect);

            ConsoleRenderer.RenderDocument(
                new ViewBuilder().CreateDocument("RTF"),
                new RtfRenderTarget(File.Create(@"../../Tmp/0.rtf")) { FontSize = 16 },
                renderRect);
        }

        private class ViewBuilder : DocumentBuilder
        {
            public Document CreateDocument (string world) =>
                Create<Document>()
                    .Color(White, Black)
                    .AddChildren(
                        Create<Div>($"Hello {world} world!").Color(Red),
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
}