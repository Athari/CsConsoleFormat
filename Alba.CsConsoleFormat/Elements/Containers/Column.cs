namespace Alba.CsConsoleFormat
{
    public class Column : Element
    {
        public GridLength Width { get; set; } = GridLength.Auto;
        public int MinWidth { get; set; } = 0;
        public int MaxWidth { get; set; } = Size.Infinity;
        public int Index { get; internal set; }
        public int ActualWidth { get; internal set; }

        protected override bool CanHaveChildren => false;

        public override string ToString() => base.ToString() + $" Width={Width}({MinWidth},{ActualWidth},{(MaxWidth == Size.Infinity ? "Inf" : MaxWidth + "")})";
    }
}