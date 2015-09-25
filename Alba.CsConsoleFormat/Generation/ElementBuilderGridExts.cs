using System;
using System.Collections;

namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderGridExts
    {
        public static ElementBuilder<T> Config<T> (this ElementBuilder<T> @this, bool autoPosition)
            where T : Grid, new()
        {
            @this.Element.AutoPosition = autoPosition;
            return @this;
        }

        public static ElementBuilder<T> AddColumns<T> (this ElementBuilder<T> @this, params object[] columns)
            where T : Grid, new()
        {
            foreach (object column in columns) {
                if (column == null)
                    continue;
                var enumerable = column as IEnumerable;
                if (enumerable != null) {
                    foreach (object subchild in enumerable)
                        @this.AddColumns(subchild);
                }
                else {
                    @this.AddColumn(column);
                }
            }
            return @this;
        }

        private static void AddColumn<T> (this ElementBuilder<T> @this, object child)
            where T : Grid, new()
        {
            if (child is GridLength) {
                var gridLength = (GridLength)child;
                @this.Element.Columns.Add(new Column { Width = gridLength });
                return;
            }
            var column = child as Column;
            if (column != null) {
                @this.Element.Columns.Add(column);
                return;
            }
            var columnBuilder = child as ElementBuilder<Column>;
            if (columnBuilder != null) {
                @this.Element.Columns.Add(columnBuilder.Element);
                return;
            }
            int width;
            try {
                width = Convert.ToInt32(child);
            }
            catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException) {
                throw new ArgumentException($"Value of type '{child.GetType().Name}' cannot be converted to column.");
            }
            @this.Element.Columns.Add(new Column { Width = GridLength.Char(width) });
        }

        public static ElementBuilder<T> AtCell<T> (this ElementBuilder<T> @this,
            int? column = null, int? row = null, int? columnSpan = null, int? rowSpan = null)
            where T : BlockElement, new()
        {
            if (column != null)
                Grid.SetColumn(@this.Element, column.Value);
            if (row != null)
                Grid.SetRow(@this.Element, row.Value);
            if (columnSpan != null)
                Grid.SetColumnSpan(@this.Element, columnSpan.Value);
            if (rowSpan != null)
                Grid.SetRowSpan(@this.Element, rowSpan.Value);
            return @this;
        }

        public static ElementBuilder<T> StrokeCell<T> (this ElementBuilder<T> @this, LineThickness stroke)
            where T : BlockElement, new()
        {
            Grid.SetStroke(@this.Element, stroke);
            return @this;
        }

        public static ElementBuilder<T> StrokeCell<T> (this ElementBuilder<T> @this, LineWidth left, LineWidth top, LineWidth right, LineWidth bottom)
            where T : BlockElement, new()
        {
            Grid.SetStroke(@this.Element, new LineThickness(left, top, right, bottom));
            return @this;
        }

        public static ElementBuilder<T> StrokeCell<T> (this ElementBuilder<T> @this, LineWidth vertical, LineWidth horizontal)
            where T : BlockElement, new()
        {
            Grid.SetStroke(@this.Element, new LineThickness(vertical, horizontal));
            return @this;
        }

        public static ElementBuilder<T> StrokeCell<T> (this ElementBuilder<T> @this, LineWidth width)
            where T : BlockElement, new()
        {
            Grid.SetStroke(@this.Element, new LineThickness(width));
            return @this;
        }
    }
}