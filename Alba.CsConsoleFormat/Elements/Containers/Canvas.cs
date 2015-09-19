using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Alba.CsConsoleFormat
{
    public class Canvas : ContainerElement
    {
        public static readonly AttachedProperty<int?> LeftProperty = RegisterAttached(() => LeftProperty);
        public static readonly AttachedProperty<int?> TopProperty = RegisterAttached(() => TopProperty);
        public static readonly AttachedProperty<int?> RightProperty = RegisterAttached(() => RightProperty);
        public static readonly AttachedProperty<int?> BottomProperty = RegisterAttached(() => BottomProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetLeft (Element el) => el.GetValue(LeftProperty);

        public static void SetLeft (Element el, int? value) => el.SetValue(LeftProperty, value);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetTop (Element el) => el.GetValue(TopProperty);

        public static void SetTop (Element el, int? value) => el.SetValue(TopProperty, value);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetRight (Element el) => el.GetValue(RightProperty);

        public static void SetRight (Element el, int? value) => el.SetValue(RightProperty, value);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetBottom (Element el) => el.GetValue(BottomProperty);

        public static void SetBottom (Element el, int? value) => el.SetValue(BottomProperty, value);

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

        private static AttachedProperty<T> RegisterAttached<T> (Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T)) =>
            AttachedProperty.Register<Canvas, T>(nameExpression, defaultValue);
    }
}