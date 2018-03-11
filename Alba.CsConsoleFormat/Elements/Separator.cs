namespace Alba.CsConsoleFormat
{
    public class Separator : BlockElement
    {
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public LineWidth Stroke { get; set; } = LineWidth.Single;

        protected override bool CanHaveChildren => false;

        protected override Size MeasureOverride(Size availableSize)
        {
            int width = Stroke.ToCharWidth();
            return Orientation == Orientation.Vertical ? new Size(width, 0) : new Size(0, width);
        }

        protected override void RenderOverride(ConsoleBuffer buffer)
        {
            base.RenderOverride(buffer);
            if (Orientation == Orientation.Vertical)
                buffer.DrawVerticalLine(0, 0, RenderSize.Height, EffectiveColor, Stroke);
            else
                buffer.DrawHorizontalLine(0, 0, RenderSize.Width, EffectiveColor, Stroke);
        }
    }
}