using System;
using System.Collections;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public static class GridExts
    {
        public static T AddColumns<T>([NotNull] this T @this, [NotNull, ItemCanBeNull] params object[] columns)
            where T : Grid
        {
            if (@this == null) 
                throw new ArgumentNullException(nameof(@this));
            if (columns == null) 
                throw new ArgumentNullException(nameof(columns));
            foreach (object column in columns) {
                switch (column) {
                    case null:
                        continue;
                    case IEnumerable enumerable:
                        foreach (object subchild in enumerable)
                            @this.AddColumns(subchild);
                        break;
                    default:
                        @this.AddColumn(column);
                        break;
                }
            }
            return @this;
        }

        private static void AddColumn<T>([NotNull] this T @this, [NotNull] object column)
            where T : Grid
        {
            switch (column) {
                case GridLength length:
                    @this.Columns.Add(new Column { Width = length });
                    break;
                case Column columnElement:
                    @this.Columns.Add(columnElement);
                    break;
                default: {
                    int width;
                    try {
                        width = Convert.ToInt32(column);
                    }
                    catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException) {
                        throw new ArgumentException($"Value of type '{column.GetType().Name}' cannot be converted to column.");
                    }
                    @this.Columns.Add(new Column { Width = GridLength.Char(width) });
                    break;
                }
            }
        }
    }
}