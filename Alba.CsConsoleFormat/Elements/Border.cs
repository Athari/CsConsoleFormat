namespace Alba.CsConsoleFormat
{
    public class Border : BlockElement
    {
        public Thickness Padding { get; set; }

        public LineThickness Stroke { get; set; }

        public Border ()
        {
            Stroke = new LineThickness(LineWidth.None);
        }

        protected override Size MeasureOverride (Size availableSize)
        {
            BlockElement child = VisualChild;
            Size borderThickness = Stroke.CollapsedCharThickness + Padding.CollapsedThickness;
            if (child != null) {
                child.Measure(availableSize - borderThickness);
                return child.DesiredSize + borderThickness;
            }
            return borderThickness;
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            BlockElement child = VisualChild;
            if (child != null)
                child.Arrange(new Rect(finalSize).Deflate(Stroke.CharThickness + Padding));
            return finalSize;
        }

        public override void Render (ConsoleRenderBuffer buffer)
        {
            // TODO >> Render Border
            base.Render(buffer);
            buffer.DrawRectangle(0, 0, RenderSize.Width, RenderSize.Height, EffectiveColor, Stroke);
        }
    }
}