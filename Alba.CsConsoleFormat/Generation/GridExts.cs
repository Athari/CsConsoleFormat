using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public static class GridExts
    {
        [Obsolete("Use Grid.Columns.Add and Grid.Columns collection initializer instead.")]
        public static T AddColumns<T>([NotNull] this T @this, [NotNull, ItemCanBeNull] params object[] columns)
            where T : Grid
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));
            @this.Columns.Add(columns);
            return @this;
        }
    }
}