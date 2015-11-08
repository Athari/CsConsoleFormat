namespace Alba.CsConsoleFormat.Tests
{
    public class ElementTestsBase
    {
        protected static void RenderOn1x1 (BlockElement element)
        {
            element.GenerateVisualTree();
            element.Measure(new Size(1, 1));
            element.Arrange(new Rect(1, 1, 1, 1));
            element.Render(new ConsoleBuffer(1));
        }

        protected static string GetRenderedText (Element element, int consoleWidth)
        {
            var doc = element as Document ?? new Document { Children = { element } };
            string text = ConsoleRenderer.RenderDocumentToText(doc, new TextRenderTarget(),
                new Rect(0, 0, consoleWidth, Size.Infinity));
            return text.Length > 0 ? text.Remove(text.Length - 2) : text;
        }
    }
}