using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;

namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderGridExts
    {
        public static ElementBuilder<Grid> CreateGrid (this DocumentBuilder @this, bool autoPosition = true)
        {
            return new ElementBuilder<Grid>(new Grid {
                AutoPosition = autoPosition
            });
        }

        public static ElementBuilder<T> AddColumns<T> (this ElementBuilder<T> @this, IEnumerable<Column> columns)
            where T : Grid, new()
        {
            @this.Element.Columns.AddRange(columns);
            return @this;
        }

        public static ElementBuilder<T> AddColumns<T> (this ElementBuilder<T> @this, params Column[] columns)
            where T : Grid, new()
        {
            return AddColumns(@this, columns.AsEnumerable());
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
    }
}