using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class Cell : Div
    {
        public Cell() : this(null)
        { }

        public Cell([CanBeNull] string text) : base(text)
        {
            Stroke = LineThickness.Single;
        }

        public int Column
        {
            get => Grid.GetColumn(this);
            set => Grid.SetColumn(this, value);
        }

        public int Row
        {
            get => Grid.GetRow(this);
            set => Grid.SetRow(this, value);
        }

        public int ColumnSpan
        {
            get => Grid.GetColumnSpan(this);
            set => Grid.SetColumnSpan(this, value);
        }

        public int RowSpan
        {
            get => Grid.GetRowSpan(this);
            set => Grid.SetRowSpan(this, value);
        }

        public LineThickness Stroke
        {
            get => Grid.GetStroke(this);
            set => Grid.SetStroke(this, value);
        }

        public override string ToString() => base.ToString() + $" Pos=({Column} {Row} {ColumnSpan} {RowSpan})";
    }
}