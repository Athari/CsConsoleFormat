namespace Alba.CsConsoleFormat
{
    public class Grid : ContainerElement
    {
        public static readonly AttachedProperty<int> ColumnProperty = AttachedProperty.Register<Canvas, int>(() => ColumnProperty);
        public static readonly AttachedProperty<int> RowProperty = AttachedProperty.Register<Canvas, int>(() => RowProperty);
        public static readonly AttachedProperty<int> ColumnSpanProperty = AttachedProperty.Register<Canvas, int>(() => ColumnSpanProperty, 1);
        public static readonly AttachedProperty<int> RowSpanProperty = AttachedProperty.Register<Canvas, int>(() => RowSpanProperty, 1);
        public static readonly AttachedProperty<LineThickness> StrokeProperty = AttachedProperty.Register<Canvas, LineThickness>(() => StrokeProperty);

        public ElementCollection Columns { get; private set; }

        public Grid ()
        {
            Columns = new ElementCollection(this);
        }

        public static int GetColumn (Element el)
        {
            return el.GetValue(ColumnProperty);
        }

        public static void SetColumn (Element el, int value)
        {
            el.SetValue(ColumnProperty, value);
        }

        public static int GetRow (Element el)
        {
            return el.GetValue(RowProperty);
        }

        public static void SetRow (Element el, int value)
        {
            el.SetValue(RowProperty, value);
        }

        public static int GetColumnSpan (Element el)
        {
            return el.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan (Element el, int value)
        {
            el.SetValue(ColumnSpanProperty, value);
        }

        public static int GetRowSpan (Element el)
        {
            return el.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan (Element el, int value)
        {
            el.SetValue(RowSpanProperty, value);
        }

        public static LineThickness GetStroke (Element el)
        {
            return el.GetValue(StrokeProperty);
        }

        public static void SetStroke (Element el, LineThickness value)
        {
            el.SetValue(StrokeProperty, value);
        }

        protected override Size MeasureOverride (Size availableSize)
        {
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            return finalSize;
        }
    }
}