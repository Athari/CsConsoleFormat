using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public class Canvas : ContainerElement
    {
        public static AttachedProperty<int?> LeftProperty = AttachedProperty.Register<Canvas, int?>(() => LeftProperty);
        public static AttachedProperty<int?> TopProperty = AttachedProperty.Register<Canvas, int?>(() => TopProperty);
        public static AttachedProperty<int?> RightProperty = AttachedProperty.Register<Canvas, int?>(() => RightProperty);
        public static AttachedProperty<int?> BottomProperty = AttachedProperty.Register<Canvas, int?>(() => BottomProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetLeft (Element el)
        {
            return el.GetValue(LeftProperty);
        }

        public static void SetLeft (Element el, int? value)
        {
            el.SetValue(LeftProperty, value);
        }

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetTop (Element el)
        {
            return el.GetValue(TopProperty);
        }

        public static void SetTop (Element el, int? value)
        {
            el.SetValue(TopProperty, value);
        }

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetRight (Element el)
        {
            return el.GetValue(RightProperty);
        }

        public static void SetRight (Element el, int? value)
        {
            el.SetValue(RightProperty, value);
        }

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetBottom (Element el)
        {
            return el.GetValue(BottomProperty);
        }

        public static void SetBottom (Element el, int? value)
        {
            el.SetValue(BottomProperty, value);
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size MeasureOverride (Size availableSize)
        {
            var childAvailableSize = new Size(Size.Infinity, Size.Infinity);
            foreach (BlockElement child in VisualChildren)
                child.Measure(childAvailableSize);
            return new Size(0, 0);
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size ArrangeOverride (Size finalSize)
        {
            foreach (BlockElement child in VisualChildren) {
                int x = 0, y = 0;

                // Compute offset of the child:
                // If Left is specified, then Right is ignored.
                // If Left is not specified, then Right is used.
                // If both are not there, then 0.
                int? left = GetLeft(child);
                if (left != null)
                    x = left.Value;
                else {
                    int? right = GetRight(child);
                    if (right != null)
                        x = finalSize.Width - child.DesiredSize.Width - right.Value;
                }

                int? top = GetTop(child);
                if (top != null)
                    y = top.Value;
                else {
                    int? bottom = GetBottom(child);
                    if (bottom != null)
                        y = finalSize.Height - child.DesiredSize.Height - bottom.Value;
                }

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
            }
            return finalSize;
        }
    }
}