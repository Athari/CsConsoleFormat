using System;
using System.Collections;

namespace Alba.CsConsoleFormat
{
    public static class GridExts
    {
        public static T AddColumns<T> (this T @this, params object[] columns)
            where T : Grid
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

        private static void AddColumn<T> (this T @this, object child)
            where T : Grid
        {
            if (child is GridLength) {
                var gridLength = (GridLength)child;
                @this.Columns.Add(new Column { Width = gridLength });
                return;
            }
            var column = child as Column;
            if (column != null) {
                @this.Columns.Add(column);
                return;
            }
            int width;
            try {
                width = Convert.ToInt32(child);
            }
            catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException) {
                throw new ArgumentException($"Value of type '{child.GetType().Name}' cannot be converted to column.");
            }
            @this.Columns.Add(new Column { Width = GridLength.Char(width) });
        }
    }
}