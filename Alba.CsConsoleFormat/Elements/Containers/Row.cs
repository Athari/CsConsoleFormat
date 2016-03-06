namespace Alba.CsConsoleFormat
{
    internal class Row : Element
    {
        public GridLength Height { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int Index { get; internal set; }
        public int ActualHeight { get; set; }

        public Row()
        {
            Height = GridLength.Auto;
            MinHeight = 0;
            MaxHeight = Size.Infinity;
        }

        protected override bool CanHaveChildren => false;

        public override string ToString() => base.ToString() + $" Height={Height}({MinHeight},{ActualHeight},{(MaxHeight == Size.Infinity ? "Inf" : MaxHeight + "")})";
    }
}