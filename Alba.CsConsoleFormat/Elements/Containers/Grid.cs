using System;
using System.Linq.Expressions;

namespace Alba.CsConsoleFormat
{
    public class Grid : ContainerElement
    {
        public static readonly AttachedProperty<int> ColumnProperty = RegisterAttached(() => ColumnProperty);
        public static readonly AttachedProperty<int> RowProperty = RegisterAttached(() => RowProperty);
        public static readonly AttachedProperty<int> ColumnSpanProperty = RegisterAttached(() => ColumnSpanProperty, 1);
        public static readonly AttachedProperty<int> RowSpanProperty = RegisterAttached(() => RowSpanProperty, 1);
        public static readonly AttachedProperty<LineThickness> StrokeProperty = RegisterAttached(() => StrokeProperty);

        public ElementCollection Columns { get; }

        public Grid ()
        {
            Columns = new ElementCollection(this);
        }

        public static int GetColumn (Element el) => el.GetValue(ColumnProperty);
        public static void SetColumn (Element el, int value) => el.SetValue(ColumnProperty, value);
        public static int GetRow (Element el) => el.GetValue(RowProperty);
        public static void SetRow (Element el, int value) => el.SetValue(RowProperty, value);
        public static int GetColumnSpan (Element el) => el.GetValue(ColumnSpanProperty);
        public static void SetColumnSpan (Element el, int value) => el.SetValue(ColumnSpanProperty, value);
        public static int GetRowSpan (Element el) => el.GetValue(RowSpanProperty);
        public static void SetRowSpan (Element el, int value) => el.SetValue(RowSpanProperty, value);
        public static LineThickness GetStroke (Element el) => el.GetValue(StrokeProperty);
        public static void SetStroke (Element el, LineThickness value) => el.SetValue(StrokeProperty, value);

        protected override Size MeasureOverride (Size availableSize)
        {
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            return finalSize;
        }

        private static AttachedProperty<T> RegisterAttached<T> (Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T)) =>
            AttachedProperty.Register<Grid, T>(nameExpression, defaultValue);
    }
}