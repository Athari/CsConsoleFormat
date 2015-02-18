using System;
using System.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConsoleRenderer
    {
        private Rect _renderRect;

        public Rect RenderRect { get; set; }

        public ConsoleRenderer ()
        {
            RenderRect = new Rect(0, 0, Console.BufferWidth, Size.Infinity);
        }

        public static Size ConsoleBufferSize
        {
            get { return new Size(Console.BufferWidth, Console.BufferHeight); }
        }

        public static Point ConsoleCursorPosition
        {
            get { return new Point(Console.CursorLeft, Console.CursorTop); }
            set
            {
                Console.CursorLeft = value.X;
                Console.CursorTop = value.Y;
            }
        }

        public static Rect ConsoleWindowRect
        {
            get { return new Rect(Console.WindowLeft, Console.WindowTop, Console.WindowWidth, Console.WindowHeight); }
            set
            {
                Console.SetWindowPosition(value.Left, value.Top);
                Console.SetWindowSize(value.Width, value.Height);
            }
        }

        public static Size ConsoleLargestWindowSize
        {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
        }

        public void RenderDocument (Document document)
        {
            _renderRect = RenderRect;
            document.GenerateVisualTree();
            document.Measure(_renderRect.Size);
            document.Arrange(_renderRect);

            var buffer = new ConsoleRenderBuffer(_renderRect.Size.Width);
            RenderElement(document, buffer, new Vector(0, 0));

            buffer.RenderToConsole();
        }

        private void RenderElement (BlockElement element, ConsoleRenderBuffer buffer, Vector parentOffset)
        {
            Vector offset = parentOffset + element.ActualOffset;
            if (element.Visibility == Visibility.Visible && !element.RenderSize.IsEmpty) {
                buffer.Clip = new Rect(element.RenderSize).Intersect(element.LayoutClip).Offset(offset).Intersect(_renderRect);
                buffer.Offset = offset;
                element.Render(buffer);

                foreach (BlockElement childElement in element.VisualChildren.OfType<BlockElement>())
                    RenderElement(childElement, buffer, offset);
            }
        }
    }
}