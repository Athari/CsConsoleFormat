namespace Alba.CsConsoleFormat
{
    public class Cell : Div
    {
        public LineThickness Stroke { get; set; }

        public int Column
        {
            get { return Grid.GetColumn(this); }
            set { Grid.SetColumn(this, value); }
        }

        public int Row
        {
            get { return Grid.GetRow(this); }
            set { Grid.SetRow(this, value); }
        }

        public int ColumnSpan
        {
            get { return Grid.GetColumnSpan(this); }
            set { Grid.SetColumnSpan(this, value); }
        }

        public int RowSpan
        {
            get { return Grid.GetRowSpan(this); }
            set { Grid.SetRowSpan(this, value); }
        }
    }
}