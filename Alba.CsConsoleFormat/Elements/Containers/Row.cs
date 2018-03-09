namespace Alba.CsConsoleFormat
{
    internal class Row : Element
    {
        public GridLength Height { get; set; } = GridLength.Auto;
        public int MinHeight { get; set; } = 0;
        public int MaxHeight { get; set; } = Size.Infinity;
        public int Index { get; internal set; }
        public int ActualHeight { get; set; }

        protected override bool CanHaveChildren => false;

        public override string ToString() => base.ToString() + $" Height={Height}({MinHeight},{ActualHeight},{(MaxHeight == Size.Infinity ? "Inf" : MaxHeight + "")})";
    }
}