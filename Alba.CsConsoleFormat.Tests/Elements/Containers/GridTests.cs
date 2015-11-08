using System;
using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class GridTests : ElementTestsBase
    {
        [Fact]
        public void NoColumnsNoChildren ()
        {
            var grid = new Grid();

            new Action(() => RenderOn1x1(grid)).ShouldNotThrow();
        }

        [Fact]
        public void NoColumnsOneChild ()
        {
            var grid = new Grid { Children = { new Div() } };

            new Action(() => RenderOn1x1(grid)).ShouldNotThrow();
        }

        [Fact]
        public void NoChildrenOneColumn ()
        {
            var grid = new Grid { Columns = { new Column() } };

            new Action(() => RenderOn1x1(grid)).ShouldNotThrow();
        }

        public static readonly object[][] ColumnConfigsData = {
            // Absolute - no borders
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(0))
                .Within(new Size(3, 1))
                .ExpectColumnWidths(0),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(3))
                .Within(new Size(3, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(3))
                .Within(new Size(4, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(3))
                .Within(new Size(2, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(0), GridLength.Char(2), GridLength.Char(3))
                .Within(new Size(5, 1))
                .ExpectColumnWidths(0, 2, 3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(1), GridLength.Char(2), GridLength.Char(3))
                .WithCells("11", "222", "3333")
                .Within(new Size(6, 1))
                .ExpectColumnWidths(1, 2, 3),
            // Star - no borders
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(0))
                .Within(new Size(3, 1))
                .ExpectColumnWidths(0),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(1))
                .Within(new Size(3, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(2))
                .Within(new Size(3, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(1), GridLength.Star(2), GridLength.Star(3))
                .Within(new Size(12, 1))
                .ExpectColumnWidths(2, 4, 6),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(0), GridLength.Star(2), GridLength.Star(3))
                .WithCells("111", "22222", "3333333")
                .Within(new Size(10, 1))
                .ExpectColumnWidths(0, 4, 6),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(1), GridLength.Star(2), GridLength.Star(3))
                .Within(new Size(14, 1))
                .ExpectColumnWidths(2, 5, 7),
            // Auto - no borders
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto)
                .WithCells("")
                .Within(new Size(1, 1))
                .ExpectColumnWidths(0),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto)
                .WithCells("123")
                .Within(new Size(2, 1))
                .ExpectColumnWidths(2),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto)
                .WithCells("123")
                .Within(new Size(3, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto)
                .WithCells("123")
                .Within(new Size(4, 1))
                .ExpectColumnWidths(3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto, GridLength.Auto, GridLength.Auto)
                .WithCells("1", "2", "3")
                .Within(new Size(4, 1))
                .ExpectColumnWidths(1, 1, 1),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto, GridLength.Auto, GridLength.Auto)
                .WithCells("1", "22", "333")
                .Within(new Size(5, 1))
                .ExpectColumnWidths(1, 2, 2),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto, GridLength.Auto, GridLength.Auto)
                .WithCells("1", "22", "333")
                .WithCells(new Div { Children = { "11111" }, [Grid.ColumnSpanProperty] = 3 })
                .Within(new Size(6, 1))
                .ExpectColumnWidths(1, 2, 3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Auto, GridLength.Auto)
                .WithCells(new Div { Children = { "1\n1\n1" }, [Grid.RowSpanProperty] = 2 })
                .WithCells("22", "33")
                .Within(new Size(4, 1))
                .ExpectColumnWidths(1, 2),
            // Mixed
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Char(1), GridLength.Star(1), GridLength.Char(3))
                .Within(new Size(8, 1))
                .ExpectColumnWidths(1, 4, 3),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(1), GridLength.Char(2), GridLength.Star(3))
                .Within(new Size(10, 1))
                .ExpectColumnWidths(2, 2, 6),
            GridConfig.WithoutBorders
                .WithColumns(GridLength.Star(1), GridLength.Auto, GridLength.Star(3))
                .WithCells("111", "222", "333")
                .Within(new Size(11, 1))
                .ExpectColumnWidths(2, 3, 6),
        };

        [Theory]
        [MemberData (nameof(ColumnConfigsData))]
        public void ColumnConfigs (GridConfig config)
        {
            var grid = new Grid { Stroke = config.GridStroke };
            foreach (Column column in config.Columns) {
                grid.Columns.Add(column);
                if (config.Cells == null)
                    grid.Children.Add(new Cell { Stroke = config.CellStroke });
            }
            if (config.Cells != null) {
                foreach (Element cell in config.Cells)
                    grid.Children.Add(cell);
            }

            grid.GenerateVisualTree();
            grid.Measure(config.Size);
            grid.Arrange(new Rect(config.Size));
            grid.Render(new ConsoleBuffer(80));

            grid.Columns.Select(c => c.ActualWidth).Should().Equal(config.ExpectedColumnWidths);
        }

        [Fact]
        public void RenderDefaultBorders ()
        {
            var grid = new Grid()
                .AddColumns(GridLength.Char(1), GridLength.Char(1), GridLength.Char(1))
                .AddChildren(Enumerable.Range(1, 6).Select(i => new Cell().AddChildren(i)));

            GetRenderedText(grid, 8).Should().BeLines(
                "╔═╤═╤═╗ ",
                "║1│2│3║ ",
                "╟─┼─┼─╢ ",
                "║4│5│6║ ",
                "╚═╧═╧═╝ ");
        }

        [Fact]
        public void RenderCustomBorders ()
        {
            var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Wide);
            var grid = new Grid { Stroke = new LineThickness(LineWidth.None) }
                .AddColumns(GridLength.Char(1), GridLength.Char(1), GridLength.Char(1))
                .AddChildren(
                    new Cell { Stroke = headerThickness }.AddChildren(1),
                    new Cell { Stroke = headerThickness }.AddChildren(2),
                    new Cell { Stroke = headerThickness }.AddChildren(3),
                    new Cell().AddChildren(4),
                    new Cell().AddChildren(5),
                    new Cell().AddChildren(6)
                );

            GetRenderedText(grid, 8).Should().BeLines(
                "╒═╤═╤═╕ ",
                "│1│2│3│ ",
                "╞═╪═╪═╡ ",
                "│4│5│6│ ",
                "└─┴─┴─┘ ");
        }

        [Fact]
        public void RenderManualPosition ()
        {
            var grid = new Grid { AutoPosition = false }
                .AddColumns(GridLength.Char(1), GridLength.Char(1), GridLength.Char(1), GridLength.Char(1))
                .AddChildren(
                    new Cell { [Grid.ColumnProperty] = 0, [Grid.RowProperty] = 0 }.AddChildren(1),
                    new Cell { [Grid.ColumnProperty] = 3, [Grid.RowProperty] = 0 }.AddChildren(2),
                    new Cell { [Grid.ColumnProperty] = 1, [Grid.RowProperty] = 1, [Grid.ColumnSpanProperty] = 3 }.AddChildren(3)
                );

            GetRenderedText(grid, 8).Should().BeLines(
                "╔═╤══╤═╗",
                "║1│  │2║",
                "╟─┼──┴─╢",
                "║ │3   ║",
                "╚═╧════╝");
        }

        public class GridConfig
        {
            public List<Column> Columns { get; set; }
            public List<Element> Cells { get; set; }
            public LineThickness GridStroke { get; set; } = LineThickness.None;
            public LineThickness CellStroke { get; set; } = LineThickness.None;
            public Size Size { get; set; }

            public List<int> ExpectedColumnWidths { get; set; }

            public static GridConfig WithoutBorders => new GridConfig { GridStroke = LineThickness.None, CellStroke = LineThickness.None };
            public static GridConfig WithBorders => new GridConfig { GridStroke = LineThickness.Single, CellStroke = LineThickness.Single };

            public GridConfig WithColumns (params GridLength[] columnWidths)
            {
                Columns = columnWidths.Select(w => new Column { Width = w }).ToList();
                return this;
            }

            public GridConfig Within (Size size)
            {
                Size = size;
                return this;
            }

            public GridConfig WithCells (params string[] cellTexts)
            {
                EnsureCells();
                foreach (string cellText in cellTexts) {
                    Cells.Add(new Cell {
                        Children = { cellText },
                        Stroke = CellStroke,
                    });
                }
                return this;
            }

            public GridConfig WithCells (params Element[] cells)
            {
                EnsureCells();
                foreach (Element cell in cells)
                    Cells.Add(cell);
                return this;
            }

            public GridConfig ExpectColumnWidths (params int[] columnWidths)
            {
                ExpectedColumnWidths = columnWidths.ToList();
                return this;
            }

            private void EnsureCells ()
            {
                if (Cells == null)
                    Cells = new List<Element>();
            }

            public override string ToString () => string.Join(", ", Columns.Select(GetColumnWidthString)) + " within " + Size;

            public static implicit operator object[] (GridConfig @this) => new object[] { @this };

            private static string GetColumnWidthString (Column c) =>
                c.Width + (c.MinWidth != 0 || c.MaxWidth != Size.Infinity ? $"{c.MinWidth},{(c.MaxWidth == Size.Infinity ? "Inf" : c.MaxWidth + "")}" : "");
        }
    }
}