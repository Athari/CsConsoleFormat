namespace Alba.CsConsoleFormat
{
    public class Div : BlockElement
    {
        public Thickness Padding { get; set; }

        public Div()
        { }

        public Div(params object[] children) : base(children)
        { }

        protected override Size MeasureOverride(Size availableSize)
        {
            BlockElement child = VisualChild;
            Size borderThickness = Padding.CollapsedThickness;
            if (child != null) {
                child.Measure(availableSize - borderThickness);
                return child.DesiredSize + borderThickness;
            }
            return borderThickness;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            VisualChild?.Arrange(new Rect(finalSize).Deflate(Padding));
            return finalSize;
        }
    }
}