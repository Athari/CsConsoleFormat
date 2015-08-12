namespace Alba.CsConsoleFormat
{
    public class Column : Element
    {
        public GridLength Width { get; set; }

        public int MinWidth { get; set; }

        public int MaxWidth { get; set; }

        public Column ()
        {
            Width = GridLength.Star(1);
            MinWidth = 0;
            MaxWidth = Size.Infinity;
        }

        protected override bool CanHaveChildren
        {
            get { return false; }
        }
    }
}