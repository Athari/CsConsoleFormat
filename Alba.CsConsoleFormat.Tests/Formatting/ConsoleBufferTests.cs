using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Tests
{
    public class ConsoleBufferTests : ElementTestsBase
    {
        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void InvalidArguments()
        {
            var buffer = new ConsoleBuffer(42);
            IList<ConsoleColor> colorMap = ColorMaps.Dark;
            IList<ConsoleColor> colorMapInvalid = new ConsoleColor[15];
            ApplyColorMapCallback processChar = (ref ConsoleChar c) => { };

            new Action(() => buffer.LineCharRenderer = null).ShouldThrow<ArgumentNullException>()
                .Which.ParamName.Should().Be("value");
            new Action(() => buffer.ApplyColorMap(new Rect(), null, processChar)).ShouldThrow<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(colorMap));
            new Action(() => buffer.ApplyColorMap(new Rect(), colorMapInvalid, processChar)).ShouldThrow<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(colorMap));
            new Action(() => buffer.ApplyColorMap(new Rect(), colorMap, null)).ShouldThrow<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(processChar));
        }

        [Fact]
        public void Create()
        {
            var buffer = new ConsoleBuffer(42);

            buffer.LineCharRenderer.Should().BeSameAs(LineCharRenderer.Box);
            buffer.Width.Should().Be(42);
            buffer.Height.Should().Be(0);
            buffer.Clip.Should().Be(new Rect(0, 0, 42, Size.Infinity));
        }

        [Fact]
        public void DrawHorizontalLine()
        {
            var buffer = new ConsoleBuffer(5);

            buffer.DrawHorizontalLine(0, 0, 5, Red);
            buffer.DrawHorizontalLine(1, 1, 3, Green);
            buffer.DrawHorizontalLine(-1, 3, 10, Blue, LineWidth.Wide);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineChar.Horizontal, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineChar.Horizontal, ForegroundColor = Green };
            var cb = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.HorizontalWide, ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, cr, cr, cr, cr);
            buffer.GetLine(1).Should().Equal(c0, cg, cg, cg, c0);
            buffer.GetLine(2).Should().Equal(c0, c0, c0, c0, c0);
            buffer.GetLine(3).Should().Equal(cb, cb, cb, cb, cb);
        }

        [Fact]
        public void DrawVerticalLine()
        {
            var buffer = new ConsoleBuffer(4);

            buffer.DrawVerticalLine(0, 0, 5, Red);
            buffer.DrawVerticalLine(1, 1, 3, Green);
            buffer.DrawVerticalLine(3, -1, 10, Blue, LineWidth.Wide);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineChar.Vertical, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineChar.Vertical, ForegroundColor = Green };
            var cb = new ConsoleChar { LineChar = LineChar.Vertical | LineChar.VerticalWide, ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, c0, c0, cb);
            buffer.GetLine(1).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(2).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(3).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(4).Should().Equal(cr, c0, c0, cb);
        }

        [Fact]
        public void DrawLine()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawLine(Line.Vertical(1, 0, 3), Red);
            buffer.DrawLine(Line.Horizontal(0, 1, 3), Green, LineWidth.Wide);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineChar.Vertical, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.HorizontalWide, ForegroundColor = Green };
            var cx = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.HorizontalWide | LineChar.Vertical, ForegroundColor = Green };

            buffer.GetLine(0).Should().Equal(c0, cr, c0);
            buffer.GetLine(1).Should().Equal(cg, cx, cg);
            buffer.GetLine(2).Should().Equal(c0, cr, c0);
        }

        [Fact]
        public void DrawRectangle()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawRectangle(new Rect(0, 0, 3, 3), Red, new LineThickness(LineWidth.Single, LineWidth.Wide));

            var c0 = new ConsoleChar();
            var cx = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.HorizontalWide | LineChar.Vertical, ForegroundColor = Red };
            var ch = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.HorizontalWide, ForegroundColor = Red };
            var cv = new ConsoleChar { LineChar = LineChar.Vertical, ForegroundColor = Red };

            buffer.GetLine(0).Should().Equal(cx, ch, cx);
            buffer.GetLine(1).Should().Equal(cv, c0, cv);
            buffer.GetLine(2).Should().Equal(cx, ch, cx);
        }

        [Fact]
        public void DrawRectangleSimple()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawRectangle(new Rect(0, 0, 3, 3), Red, LineWidth.Single);

            var c0 = new ConsoleChar();
            var cx = new ConsoleChar { LineChar = LineChar.Horizontal | LineChar.Vertical, ForegroundColor = Red };
            var ch = new ConsoleChar { LineChar = LineChar.Horizontal, ForegroundColor = Red };
            var cv = new ConsoleChar { LineChar = LineChar.Vertical, ForegroundColor = Red };

            buffer.GetLine(0).Should().Equal(cx, ch, cx);
            buffer.GetLine(1).Should().Equal(cv, c0, cv);
            buffer.GetLine(2).Should().Equal(cx, ch, cx);
        }

        [Fact]
        public void FillForegroundHorizontalLine()
        {
            var buffer = new ConsoleBuffer(5);

            buffer.FillForegroundHorizontalLine(0, 0, 5, Red, '-');
            buffer.FillForegroundHorizontalLine(1, 1, 3, Green, '=');
            buffer.FillForegroundHorizontalLine(-1, 3, 10, Blue, '_');

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { Char = '-', ForegroundColor = Red };
            var cg = new ConsoleChar { Char = '=', ForegroundColor = Green };
            var cb = new ConsoleChar { Char = '_', ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, cr, cr, cr, cr);
            buffer.GetLine(1).Should().Equal(c0, cg, cg, cg, c0);
            buffer.GetLine(2).Should().Equal(c0, c0, c0, c0, c0);
            buffer.GetLine(3).Should().Equal(cb, cb, cb, cb, cb);
        }

        [Fact]
        public void FillForegroundVerticalLine()
        {
            var buffer = new ConsoleBuffer(4);

            buffer.FillForegroundVerticalLine(0, 0, 5, Red, '|');
            buffer.FillForegroundVerticalLine(1, 1, 3, Green, ':');
            buffer.FillForegroundVerticalLine(3, -1, 10, Blue, '*');

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { Char = '|', ForegroundColor = Red };
            var cg = new ConsoleChar { Char = ':', ForegroundColor = Green };
            var cb = new ConsoleChar { Char = '*', ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, c0, c0, cb);
            buffer.GetLine(1).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(2).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(3).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(4).Should().Equal(cr, c0, c0, cb);
        }

        [Fact]
        public void FillForegroundLine()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.FillForegroundLine(Line.Vertical(1, 0, 3), Red, '|');
            buffer.FillForegroundLine(Line.Horizontal(0, 1, 3), Green, '-');

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { Char = '|', ForegroundColor = Red };
            var cg = new ConsoleChar { Char = '-', ForegroundColor = Green };

            buffer.GetLine(0).Should().Equal(c0, cr, c0);
            buffer.GetLine(1).Should().Equal(cg, cg, cg);
            buffer.GetLine(2).Should().Equal(c0, cr, c0);
        }

        [Fact]
        public void FillBackgroundHorizontalLine()
        {
            var buffer = new ConsoleBuffer(5);

            buffer.FillBackgroundHorizontalLine(0, 0, 5, Red);
            buffer.FillBackgroundHorizontalLine(1, 1, 3, Green);
            buffer.FillBackgroundHorizontalLine(-1, 3, 10, Blue);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { BackgroundColor = Red };
            var cg = new ConsoleChar { BackgroundColor = Green };
            var cb = new ConsoleChar { BackgroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, cr, cr, cr, cr);
            buffer.GetLine(1).Should().Equal(c0, cg, cg, cg, c0);
            buffer.GetLine(2).Should().Equal(c0, c0, c0, c0, c0);
            buffer.GetLine(3).Should().Equal(cb, cb, cb, cb, cb);
        }

        [Fact]
        public void FillBackgroundVerticalLine()
        {
            var buffer = new ConsoleBuffer(4);

            buffer.FillBackgroundVerticalLine(0, 0, 5, Red);
            buffer.FillBackgroundVerticalLine(1, 1, 3, Green);
            buffer.FillBackgroundVerticalLine(3, -1, 10, Blue);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { BackgroundColor = Red };
            var cg = new ConsoleChar { BackgroundColor = Green };
            var cb = new ConsoleChar { BackgroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, c0, c0, cb);
            buffer.GetLine(1).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(2).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(3).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(4).Should().Equal(cr, c0, c0, cb);
        }

        [Fact]
        public void FillBackgroundLine()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.FillBackgroundLine(Line.Vertical(1, 0, 3), Red);
            buffer.FillBackgroundLine(Line.Horizontal(0, 1, 3), Green);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { BackgroundColor = Red };
            var cg = new ConsoleChar { BackgroundColor = Green };

            buffer.GetLine(0).Should().Equal(c0, cr, c0);
            buffer.GetLine(1).Should().Equal(cg, cg, cg);
            buffer.GetLine(2).Should().Equal(c0, cr, c0);
        }

        [Fact]
        public void ApplyColorMap()
        {
            var buffer = new ConsoleBuffer(2);

            buffer.ApplyBackgroundColorMap(new Rect(0, 0, 1, 1), ColorMaps.Invert);
            buffer.ApplyForegroundColorMap(new Rect(1, 0, 1, 1), ColorMaps.Invert);

            buffer.GetLine(0).Should().Equal(
                new ConsoleChar { BackgroundColor = White },
                new ConsoleChar { ForegroundColor = White });
        }
    }
}