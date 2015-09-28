using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Alba.CsConsoleFormat.Framework.Collections;
using JetBrains.Annotations;
using static System.Linq.Enumerable;
using static System.Math;
using static Alba.CsConsoleFormat.LineWidthExts;

namespace Alba.CsConsoleFormat
{
    public class Grid : ContainerElement
    {
        public static readonly AttachedProperty<int> ColumnProperty = RegisterAttached(() => ColumnProperty);
        public static readonly AttachedProperty<int> RowProperty = RegisterAttached(() => RowProperty);
        public static readonly AttachedProperty<int> ColumnSpanProperty = RegisterAttached(() => ColumnSpanProperty, 1);
        public static readonly AttachedProperty<int> RowSpanProperty = RegisterAttached(() => RowSpanProperty, 1);
        public static readonly AttachedProperty<LineThickness> StrokeProperty = RegisterAttached(() => StrokeProperty, LineThickness.None);

        private List<List<BlockElement>> _cells;
        private List<List<LineWidth>> _rowBorders;
        private List<List<LineWidth>> _columnBorders;
        private List<int> _maxRowBorders;
        private List<int> _maxColumnBorders;

        public bool AutoPosition { get; set; } = true;
        public ElementCollection<Column> Columns { get; }
        internal ElementCollection<Row> Rows { get; }

        public Grid ()
        {
            Columns = new ElementCollection<Column>(this);
            Rows = new ElementCollection<Row>(this);
            Stroke = LineThickness.Wide;
        }

        public LineThickness Stroke
        {
            get { return GetStroke(this); }
            set { SetStroke(this, value); }
        }

        public static int GetColumn ([NotNull] BlockElement @this) => @this.GetValueSafe(ColumnProperty);
        public static int GetRow ([NotNull] BlockElement @this) => @this.GetValueSafe(RowProperty);
        public static int GetColumnSpan ([NotNull] BlockElement @this) => @this.GetValueSafe(ColumnSpanProperty);
        public static int GetRowSpan ([NotNull] BlockElement @this) => @this.GetValueSafe(RowSpanProperty);
        public static LineThickness GetStroke ([NotNull] BlockElement @this) => @this.GetValueSafe(StrokeProperty);
        public static void SetColumn ([NotNull] BlockElement @this, int value) => @this.SetValueSafe(ColumnProperty, value);
        public static void SetRow ([NotNull] BlockElement @this, int value) => @this.SetValueSafe(RowProperty, value);
        public static void SetColumnSpan ([NotNull] BlockElement @this, int value) => @this.SetValueSafe(ColumnSpanProperty, value);
        public static void SetRowSpan ([NotNull] BlockElement @this, int value) => @this.SetValueSafe(RowSpanProperty, value);
        public static void SetStroke ([NotNull] BlockElement @this, LineThickness value) => @this.SetValueSafe(StrokeProperty, value);

        protected override void SetVisualChildren (IList<Element> visualChildren)
        {
            base.SetVisualChildren(visualChildren);
            if (Columns.Count == 0)
                return;
            CalculateCellPositions();
            PrepareColumnsAndRows();
            CalculateBorders();
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        private void CalculateCellPositions ()
        {
            _cells = new List<List<BlockElement>>();
            int autoColumn = 0, autoRow = 0;
            foreach (BlockElement cell in VisualChildren) {
                int cellColumnSpan = GetColumnSpan(cell), cellRowSpan = GetRowSpan(cell);
                if (cellColumnSpan > Columns.Count) {
                    SetColumnSpan(cell, Columns.Count);
                    cellColumnSpan = Columns.Count;
                }
                if (AutoPosition) {
                    // TODO Optimize search for empty span.
                    while (autoColumn + cellColumnSpan > Columns.Count || IsAnyCellAtSpan(autoColumn, autoRow, cellColumnSpan, cellRowSpan)) {
                        autoColumn++;
                        if (autoColumn >= Columns.Count) {
                            autoColumn = 0;
                            autoRow++;
                        }
                    }
                    SetColumn(cell, autoColumn);
                    SetRow(cell, autoRow);
                }
                int cellColumn = GetColumn(cell), cellRow = GetRow(cell);
                AddCellToSpan(cell, cellColumn, cellRow, cellColumnSpan, cellRowSpan);
            }
        }

        private void PrepareColumnsAndRows ()
        {
            for (int column = 0; column < Columns.Count; column++)
                Columns[column].Index = column;
            Rows.AddRange(Range(0, _cells.Count).Select(row => new Row { Index = row }));
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        private void CalculateBorders ()
        {
            // columns: 5  rows: 2  column borders: 2x6  row borders: 3x5
            // pos: 1x2 3x1
            // +-+-+-+-+-+
            // | | | | | |
            // +-+-+-+-+-+
            // | | |$|$|$|
            // +-+-+-+-+-+

            // Initialize with empty cell borders.
            int ncolumns = Columns.Count, nrows = _cells.Count;
            _columnBorders = ListOfListOf(nrows, ncolumns + 1, LineWidth.None);
            _rowBorders = ListOfListOf(nrows + 1, ncolumns, LineWidth.None);
            _maxColumnBorders = ListOf(ncolumns + 1, 0);
            _maxRowBorders = ListOf(nrows + 1, 0);

            // Apply table borders (Stroke property of grid).
            LineThickness tableStroke = Stroke;
            for (int row = 0; row < nrows; row++) {
                MergeBorderWidth(_columnBorders, row, 0, tableStroke.Left);
                MergeBorderWidth(_columnBorders, row, ncolumns, tableStroke.Right);
            }
            for (int column = 0; column < ncolumns; column++) {
                MergeBorderWidth(_rowBorders, 0, column, tableStroke.Top);
                MergeBorderWidth(_rowBorders, nrows, column, tableStroke.Bottom);
            }

            // Apply cell borders (attached Stroke property of cells).
            foreach (BlockElement cell in VisualChildren) {
                int cellColumn = GetColumn(cell), cellRow = GetRow(cell);
                int cellColumnSpan = GetColumnSpan(cell), cellRowSpan = GetRowSpan(cell);
                LineThickness cellStroke = GetStroke(cell);

                for (int row = 0; row < cellRowSpan; row++) {
                    MergeBorderWidth(_columnBorders, cellRow + row, cellColumn, cellStroke.Left);
                    MergeBorderWidth(_columnBorders, cellRow + row, cellColumn + cellColumnSpan, cellStroke.Right);
                }
                for (int column = 0; column < cellColumnSpan; column++) {
                    MergeBorderWidth(_rowBorders, cellRow, cellColumn + column, cellStroke.Top);
                    MergeBorderWidth(_rowBorders, cellRow + cellRowSpan, cellColumn + column, cellStroke.Bottom);
                }
            }

            // Calculate max char widths.
            for (int column = 0; column < ncolumns + 1; column++)
                for (int row = 0; row < nrows; row++)
                    _maxColumnBorders[column] = Max(_maxColumnBorders[column], _columnBorders[row][column].ToCharWidth());
            for (int row = 0; row < nrows + 1; row++)
                for (int column = 0; column < ncolumns; column++)
                    _maxRowBorders[row] = Max(_maxRowBorders[row], _rowBorders[row][column].ToCharWidth());
        }

        protected override Size MeasureOverride (Size availableSize)
        {
            if (Columns.Count == 0)
                return new Size(0, 0);

            Size borderSize = new Size(_maxColumnBorders.Sum(), _maxRowBorders.Sum());
            int availableWidth = Max(availableSize.Width - borderSize.Width, 0);

            MeasureAbsoluteColumns(ref availableWidth);
            MeasureAutoColumns(ref availableWidth);
            MeasureStarColumns(ref availableWidth);
            MeasureSpannedCells();
            // TODO > Remeasure cells if not enough height.

            return new Size(
                Columns.Sum(c => c.ActualWidth) + borderSize.Width,
                Rows.Sum(c => c.ActualHeight) + borderSize.Height);
        }

        private void MeasureAbsoluteColumns (ref int availableWidth)
        {
            foreach (Column gridColumn in Columns.Where(c => c.Width.IsAbsolute)) {
                gridColumn.ActualWidth = MinMax(gridColumn.Width.Value, gridColumn.MinWidth, gridColumn.MaxWidth);
                Size maxCellSize = new Size(Min(gridColumn.ActualWidth, availableWidth), Size.Infinity);
                foreach (Row gridRow in Rows) {
                    BlockElement cell = _cells[gridRow.Index][gridColumn.Index];
                    if (cell != null && GetColumnSpan(cell) == 1 && GetRowSpan(cell) == 1) {
                        cell.Measure(maxCellSize);
                        gridRow.ActualHeight = Max(gridRow.ActualHeight, cell.DesiredSize.Height);
                    }
                }
                availableWidth = Max(availableWidth - gridColumn.ActualWidth, 0);
            }
        }

        private void MeasureAutoColumns (ref int availableWidth)
        {
            foreach (Column gridColumn in Columns.Where(c => c.Width.IsAuto)) {
                gridColumn.ActualWidth = gridColumn.MinWidth;
                Size maxCellSize = new Size(availableWidth, Size.Infinity);
                foreach (Row gridRow in Rows) {
                    BlockElement cell = _cells[gridRow.Index][gridColumn.Index];
                    if (cell != null && GetColumnSpan(cell) == 1 && GetRowSpan(cell) == 1) {
                        cell.Measure(maxCellSize);
                        gridColumn.ActualWidth = MinMax(gridColumn.ActualWidth, cell.DesiredSize.Width, gridColumn.MaxWidth);
                        gridRow.ActualHeight = Max(gridRow.ActualHeight, cell.DesiredSize.Height);
                    }
                }
                availableWidth = Max(availableWidth - gridColumn.ActualWidth, 0);
            }
        }

        private void MeasureStarColumns (ref int availableWidth)
        {
            List<Column> starColumns = Columns.Where(c => c.Width.IsStar).ToList();
            if (starColumns.Count > 0) {
                // Distribute widths according to weights.
                List<int> weights = starColumns.Select(c => c.Width.Value).ToList();
                if (availableWidth == 0 || weights.Sum() == 0) {
                    foreach (Column gridColumn in starColumns)
                        gridColumn.ActualWidth = 0;
                }
                else {
                    List<int> widths = Distribute(weights.Select(w => (double)w).ToList(), availableWidth);
                    for (int i = 0; i < starColumns.Count; i++)
                        starColumns[i].ActualWidth = widths[i];
                }
                // Treat star columns like absolute columns.
                foreach (Column gridColumn in starColumns) {
                    // TODO Respect WinWidth and MaxWidth during distributing values, not apply contraints later.
                    gridColumn.ActualWidth = MinMax(gridColumn.ActualWidth, gridColumn.MinWidth, gridColumn.MaxWidth);
                    Size maxCellSize = new Size(gridColumn.ActualWidth, Size.Infinity);
                    foreach (Row gridRow in Rows) {
                        BlockElement cell = _cells[gridRow.Index][gridColumn.Index];
                        if (cell != null && GetColumnSpan(cell) == 1 && GetRowSpan(cell) == 1) {
                            cell.Measure(maxCellSize);
                            gridRow.ActualHeight = Max(gridRow.ActualHeight, cell.DesiredSize.Height);
                        }
                    }
                    availableWidth = Max(availableWidth - gridColumn.ActualWidth, 0);
                }
            }
        }

        private void MeasureSpannedCells ()
        {
            foreach (BlockElement cell in VisualChildren.Cast<BlockElement>().OrderBy(GetCellMeasurePriority)) {
                int cellColumnSpan = GetColumnSpan(cell), cellRowSpan = GetRowSpan(cell);
                if (cellColumnSpan == 1 && cellRowSpan == 1)
                    continue;
                int cellColumn = GetColumn(cell), cellRow = GetRow(cell);
                Size cellSpannedSize = GetSpannedCellSize(cellColumn, cellRow, cellColumnSpan, cellRowSpan);
                cell.Measure(new Size(cellSpannedSize.Width, Size.Infinity));
                // TODO Support column spans too.
                // Distribute extra height evenly between rows.
                if (cell.DesiredSize.Height > cellSpannedSize.Height) {
                    int extraHeight = cell.DesiredSize.Height - cellSpannedSize.Height;
                    if (cellRowSpan == 1) {
                        Rows[cellRow].ActualHeight += extraHeight;
                    }
                    else {
                        List<Row> cellRows = Rows.Skip(cellRow).Take(cellRowSpan).ToList();
                        List<double> weights = cellRows.Select(r => Max(r.ActualHeight, 0.0001)).ToList();
                        List<int> extraRowHeights = Distribute(weights, extraHeight);
                        for (int row = 0; row < cellRows.Count; row++)
                            cellRows[row].ActualHeight += extraRowHeights[row];
                    }
                }
            }
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            var cellRect = new Rect { Y = _maxRowBorders[0] };
            foreach (Row gridRow in Rows) {
                cellRect.Height = gridRow.ActualHeight;
                cellRect.X = _maxColumnBorders[0];
                foreach (Column gridColumn in Columns) {
                    cellRect.Width = gridColumn.ActualWidth;
                    ArrangeCell(gridRow, gridColumn, cellRect);
                    cellRect.X += gridColumn.ActualWidth + _maxColumnBorders[gridColumn.Index + 1];
                }
                cellRect.Y += gridRow.ActualHeight + _maxRowBorders[gridRow.Index + 1];
            }
            return finalSize;
        }

        private void ArrangeCell (Row gridRow, Column gridColumn, Rect cellRect)
        {
            BlockElement cell = _cells[gridRow.Index][gridColumn.Index];
            if (cell == null)
                return;

            int cellColumn = GetColumn(cell), cellRow = GetRow(cell);
            int cellColumnSpan = GetColumnSpan(cell), cellRowSpan = GetRowSpan(cell);
            // Cell rect is only valid for 1x1 cells, calculate size for spanned cells.
            if (cellColumnSpan > 1 || cellRowSpan > 1) {
                // Arrange only one time in the first cell.
                if (cellColumn != gridColumn.Index || cellRow != gridRow.Index)
                    return;
                cellRect.Size = GetSpannedCellSize(cellColumn, cellRow, cellColumnSpan, cellRowSpan);
            }
            cell.Arrange(cellRect);
        }

        public override void Render (ConsoleBuffer buffer)
        {
            base.Render(buffer);
            // Draw borders.
            var borderRect = new Rect();
            foreach (Row gridRow in Rows) {
                borderRect.Height = gridRow.ActualHeight + _maxRowBorders[gridRow.Index] + _maxRowBorders[gridRow.Index + 1];
                borderRect.X = 0;
                foreach (Column gridColumn in Columns) {
                    borderRect.Width = gridColumn.ActualWidth + _maxColumnBorders[gridColumn.Index] + _maxColumnBorders[gridColumn.Index + 1];
                    buffer.DrawRectangle(borderRect, EffectiveColor,
                        new LineThickness(
                            _columnBorders[gridRow.Index][gridColumn.Index],
                            _rowBorders[gridRow.Index][gridColumn.Index],
                            _columnBorders[gridRow.Index][gridColumn.Index + 1],
                            _rowBorders[gridRow.Index + 1][gridColumn.Index]));
                    borderRect.X += gridColumn.ActualWidth + _maxColumnBorders[gridColumn.Index];
                }
                borderRect.Y += gridRow.ActualHeight + _maxRowBorders[gridRow.Index];
            }
        }

        private bool IsAnyCellAtSpan (int cellColumn, int cellRow, int cellColumnSpan, int cellRowSpan)
        {
            for (int column = 0; column < cellColumnSpan; column++)
                for (int row = 0; row < cellRowSpan; row++)
                    if (GetCellAtPosition(cellColumn + column, cellRow + row) != null)
                        return true;
            return false;
        }

        private BlockElement GetCellAtPosition (int cellColumn, int cellRow)
        {
            return cellRow < _cells.Count ? _cells[cellRow][cellColumn] : null;
        }

        private void AddCellToSpan (BlockElement cell, int cellColumn, int cellRow, int cellColumnSpan, int cellRowSpan)
        {
            for (int column = 0; column < cellColumnSpan; column++)
                for (int row = 0; row < cellRowSpan; row++)
                    AddCellToPosition(cell, cellColumn + column, cellRow + row);
        }

        private void AddCellToPosition (BlockElement cell, int cellColumn, int cellRow)
        {
            while (cellRow >= _cells.Count)
                _cells.Add(ListOf<BlockElement>(Columns.Count));
            _cells[cellRow][cellColumn] = cell;
        }

        private Size GetSpannedCellSize (int cellColumn, int cellRow, int cellColumnSpan, int cellRowSpan)
        {
            return new Size(
                Columns.Skip(cellColumn).Take(cellColumnSpan).Sum(c => c.ActualWidth)
                    + _maxColumnBorders.Skip(cellColumn + 1).Take(cellColumnSpan - 1).Sum(),
                Rows.Skip(cellRow).Take(cellRowSpan).Sum(c => c.ActualHeight)
                    + _maxRowBorders.Skip(cellRow + 1).Take(cellRowSpan - 1).Sum());
        }

        private static int GetCellMeasurePriority (BlockElement cell)
        {
            int cellColumnSpan = GetColumnSpan(cell), cellRowSpan = GetRowSpan(cell);
            if (cellColumnSpan == 1 && cellRowSpan == 1)
                return 0;
            else if (cellRowSpan == 1)
                return 1;
            else
                return 2;
        }

        private static void MergeBorderWidth (List<List<LineWidth>> borders, int row, int column, LineWidth width)
        {
            borders[row][column] = Max(borders[row][column], width);
        }

        private static List<T> ListOf<T> (int count, T value = default(T))
        {
            var list = new List<T>(count);
            for (int i = 0; i < count; i++)
                list.Add(value);
            return list;
        }

        private static List<List<T>> ListOfListOf<T> (int rows, int columns, T value = default(T))
        {
            var list = new List<List<T>>(rows);
            for (int i = 0; i < rows; i++)
                list.Add(ListOf(columns, value));
            return list;
        }

        private static List<int> Distribute (List<double> weights, int available)
        {
            double totalWeight = weights.Sum();
            if (Abs(totalWeight) <= double.Epsilon)
                throw new ArgumentException("Total weight must be positive.");

            var values = new DistributedValue[weights.Count];
            for (int i = 0; i < weights.Count; i++)
                values[i] = new DistributedValue(available * weights[i] / totalWeight);
            int totalResult = values.Sum(v => v.Result);

            while (totalResult < available) {
                double maxError = 0;
                int maxErrorIndex = -1;
                for (int i = 0; i < values.Length; ++i) {
                    if (values[i].Error > maxError) {
                        maxError = values[i].Error;
                        maxErrorIndex = i;
                    }
                }
                values[maxErrorIndex].Increment();
                totalResult++;
            }

            return values.Select(v => v.Result).ToList();
        }

        private static AttachedProperty<T> RegisterAttached<T> (Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T)) =>
            AttachedProperty.Register<Grid, T>(nameExpression, defaultValue);

        private struct DistributedValue
        {
            public int Result;
            public double Error;

            public DistributedValue (double actual)
            {
                Result = (int)Floor(actual);
                Error = actual - Result;
            }

            public void Increment ()
            {
                Result++;
                Error--;
            }
        }
    }
}