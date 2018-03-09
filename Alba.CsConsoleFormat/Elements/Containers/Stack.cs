using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The name is fine, I like it.")]
    public class Stack : ContainerElement
    {
        public Orientation Orientation { get; set; } = Orientation.Vertical;

        public Stack()
        { }

        public Stack(params object[] children) : base(children)
        { }

        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Vertical) {
                int totalHeight = 0;
                int maxWidth = 0;
                foreach (BlockElement child in VisualChildren) {
                    child.Measure(availableSize);
                    totalHeight += child.DesiredSize.Height;
                    if (child.DesiredSize.Width > maxWidth)
                        maxWidth = child.DesiredSize.Width;
                }
                foreach (BlockElement child in VisualChildren)
                    child.Measure(new Size(maxWidth, child.DesiredSize.Height));
                return new Size(maxWidth, totalHeight);
            }
            else {
                int totalWidth = 0;
                int maxHeight = 0;
                foreach (BlockElement child in VisualChildren) {
                    child.Measure(availableSize);
                    totalWidth += child.DesiredSize.Width;
                    if (child.DesiredSize.Height > maxHeight)
                        maxHeight = child.DesiredSize.Height;
                }
                foreach (BlockElement child in VisualChildren)
                    child.Measure(new Size(child.DesiredSize.Width, maxHeight));
                return new Size(totalWidth, maxHeight);
            }
        }

        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Orientation == Orientation.Vertical) {
                int totalHeight = 0;
                int maxWidth = VisualChildren.Cast<BlockElement>().Select(c => c.DesiredSize.Width).Concat(finalSize.Width).Max();
                foreach (BlockElement child in VisualChildren) {
                    int height = child.DesiredSize.Height;
                    child.Arrange(new Rect(0, totalHeight, maxWidth, height));
                    totalHeight += height;
                }
                return finalSize;
            }
            else {
                int totalWidth = 0;
                int maxHeight = VisualChildren.Cast<BlockElement>().Select(c => c.DesiredSize.Height).Concat(finalSize.Height).Max();
                foreach (BlockElement child in VisualChildren) {
                    int width = child.DesiredSize.Width;
                    child.Arrange(new Rect(totalWidth, 0, width, maxHeight));
                    totalWidth += width;
                }
                return finalSize;
            }
        }
    }
}