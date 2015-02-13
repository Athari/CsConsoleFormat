namespace Alba.CsConsoleFormat
{
    public class Line : BlockElement
    {
        public Orientation Orientation { get; set; }

        public LineWidth Stroke { get; set; }

        protected override bool CanHaveChildren
        {
            get { return false; }
        }

        public Line ()
        {
            Stroke = LineWidth.Single;
            Align = HorizontalAlignment.Stretch;
        }

        protected override Size MeasureOverride (Size availableSize)
        {
            int width = Stroke.ToCharWidth();
            return Orientation == Orientation.Vertical ? new Size(width, 0) : new Size(0, width);
        }
    }
}