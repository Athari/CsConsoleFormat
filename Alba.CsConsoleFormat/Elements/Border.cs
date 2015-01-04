namespace Alba.CsConsoleFormat
{
    public class Border : BlockElement
    {
        public LineThickness Stroke { get; set; }

        public Border ()
        {
            Stroke = new LineThickness(LineWidth.Single);
        }
    }
}