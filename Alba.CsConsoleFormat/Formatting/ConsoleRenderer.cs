using System;
using System.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderer
    {
        public void RenderDocument (Document document)
        {
            var consoleSize = new Size(Console.BufferWidth, int.MaxValue);
            document.GenerateVisualTree();
            document.Measure(consoleSize);
            document.Arrange(new Rect(consoleSize));

            var buffer = new ConsoleRenderBuffer(consoleSize.Width);
            RenderElement(document, buffer);

            buffer.RenderToConsole();
        }

        private void RenderElement (BlockElement element, ConsoleRenderBuffer buffer)
        {
            if (element.Visibility == Visibility.Visible && element.RenderSize.Width > 0 && element.RenderSize.Height > 0) {
                // TODO >>>
                element.Render(buffer);
            }
            foreach (BlockElement childElement in element.VisualChildren.OfType<BlockElement>())
                RenderElement(childElement, buffer);
        }
    }
}