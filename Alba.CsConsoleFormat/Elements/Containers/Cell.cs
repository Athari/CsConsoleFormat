namespace Alba.CsConsoleFormat
{
    public class Cell : Div
    {
        public Cell ()
        {
            Stroke = LineThickness.Single;
        }

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

        public LineThickness Stroke
        {
            get { return Grid.GetStroke(this); }
            set { Grid.SetStroke(this, value); }
        }

        public override string ToString () => base.ToString() + $" Pos=({Column} {Row} {ColumnSpan} {RowSpan})";
    }
}