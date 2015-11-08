using System;
using System.Text;

namespace Alba.CsConsoleFormat.Tests
{
    public class ElementTestsBase
    {
        private char _fillChar = 'a';

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

        protected Div CreateRectDiv (int width, int height)
        {
            var sb = new StringBuilder();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++)
                    sb.Append(_fillChar++);
                sb.AppendLine();
            }
            int newLineLength = Environment.NewLine.Length;
            sb.Remove(sb.Length - newLineLength, newLineLength);

            return new Div {
                TextWrap = TextWrapping.NoWrap,
                Width = width,
                Height = height,
                Align = HorizontalAlignment.Left,
                VAlign = VerticalAlignment.Top,
                Children = {
                    new Span(sb.ToString())
                }
            };
        }
    }
}