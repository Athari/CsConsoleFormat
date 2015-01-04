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
        }
    }
}