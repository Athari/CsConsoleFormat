namespace Alba.CsConsoleFormat
{
    public class Line : BlockElement
    {
        public Orientation Orientation { get; set; }
        public LineWidth Stroke { get; set; }

        public Line ()
        {
            Stroke = LineWidth.Single;
        }
    }
}