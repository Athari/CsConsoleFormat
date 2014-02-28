namespace Alba.CsConsoleFormat
{
    public class Border : ContainerElement
    {
        public LineThickness Stroke { get; set; }

        public Border ()
        {
            Stroke = new LineThickness(LineWidth.Single);
        }
    }
}