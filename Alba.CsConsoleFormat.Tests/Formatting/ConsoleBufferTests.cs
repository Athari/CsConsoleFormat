using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class ConsoleBufferTests : ElementTestsBase
    {
        private static readonly LineChar LineCharHorizontal = new LineChar(LineWidth.Single, LineWidth.None);
        private static readonly LineChar LineCharHorizontalVertical = new LineChar(LineWidth.Single, LineWidth.Single);
        private static readonly LineChar LineCharHorizontalDouble = new LineChar(LineWidth.Double, LineWidth.None);
        private static readonly LineChar LineCharHorizontalDoubleVertical = new LineChar(LineWidth.Double, LineWidth.Single);
        private static readonly LineChar LineCharVertical = new LineChar(LineWidth.None, LineWidth.Single);
        private static readonly LineChar LineCharVerticalDouble = new LineChar(LineWidth.None, LineWidth.Double);
        private static LineChar LineCharLeft(LineWidth width) => new LineChar(width, LineWidth.None, LineWidth.None, LineWidth.None);
        private static LineChar LineCharTop(LineWidth width) => new LineChar(LineWidth.None, width, LineWidth.None, LineWidth.None);
        private static LineChar LineCharRight(LineWidth width) => new LineChar(LineWidth.None, LineWidth.None, width, LineWidth.None);
        private static LineChar LineCharBottom(LineWidth width) => new LineChar(LineWidth.None, LineWidth.None, LineWidth.None, width);
        private static LineChar LineCharTopLeft(LineWidth top, LineWidth left) => new LineChar(left, top, LineWidth.None, LineWidth.None);
        private static LineChar LineCharTopRight(LineWidth top, LineWidth right) => new LineChar(LineWidth.None, top, right, LineWidth.None);
        private static LineChar LineCharBottomLeft(LineWidth bottom, LineWidth left) => new LineChar(left, LineWidth.None, LineWidth.None, bottom);
        private static LineChar LineCharBottomRight(LineWidth bottom, LineWidth right) => new LineChar(LineWidth.None, LineWidth.None, right, bottom);

        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "ConvertToLocalFunction")]
        public void InvalidArguments()
        {
            var buffer = new ConsoleBuffer(42);
            IList<ConsoleColor> colorMap = ColorMaps.Dark;
            IList<ConsoleColor> colorMapInvalid = new ConsoleColor[15];
            ApplyColorMapCallback processChar = (ref ConsoleChar c) => { };

            new Action(() => buffer.LineCharRenderer = null).Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("value");
            new Action(() => buffer.ApplyColorMap(new Rect(), null, processChar)).Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(colorMap));
            new Action(() => buffer.ApplyColorMap(new Rect(), colorMapInvalid, processChar)).Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(colorMap));
            new Action(() => buffer.ApplyColorMap(new Rect(), colorMap, null)).Should().Throw<ArgumentNullException>()
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
        public void DrawHorizontalLineCapFull()
        {
            var buffer = new ConsoleBuffer(5);

            buffer.DrawHorizontalLine(0, 0, 5, Red, LineWidth.Single, DrawLineFlags.CapFull);
            buffer.DrawHorizontalLine(1, 1, 3, Green, LineWidth.Single, DrawLineFlags.CapFull);
            buffer.DrawHorizontalLine(-1, 3, 10, Blue, LineWidth.Double, DrawLineFlags.CapFull);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineCharHorizontal, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineCharHorizontal, ForegroundColor = Green };
            var cb = new ConsoleChar { LineChar = LineCharHorizontalDouble, ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, cr, cr, cr, cr);
            buffer.GetLine(1).Should().Equal(c0, cg, cg, cg, c0);
            buffer.GetLine(2).Should().Equal(c0, c0, c0, c0, c0);
            buffer.GetLine(3).Should().Equal(cb, cb, cb, cb, cb);
        }

        [Fact]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        public void DrawHorizontalLineCapCenter()
        {
            var buffer = new ConsoleBuffer(5);

            buffer.DrawHorizontalLine(0, 0, 5, Red, LineWidth.Single, DrawLineFlags.CapCenter);
            buffer.DrawHorizontalLine(1, 1, 3, Green, LineWidth.Single, DrawLineFlags.CapStartCenter);
            buffer.DrawHorizontalLine(2, 2, 2, Blue, LineWidth.Double, DrawLineFlags.CapEndCenter);
            buffer.DrawHorizontalLine(-1, 3, 10, Blue, LineWidth.Double, DrawLineFlags.CapCenter);

            var c0m = new ConsoleChar();
            var crs = new ConsoleChar { LineChar = LineCharRight(LineWidth.Single), ForegroundColor = Red };
            var crm = new ConsoleChar { LineChar = LineCharHorizontal, ForegroundColor = Red };
            var cre = new ConsoleChar { LineChar = LineCharLeft(LineWidth.Single), ForegroundColor = Red };
            var cgs = new ConsoleChar { LineChar = LineCharRight(LineWidth.Single), ForegroundColor = Green };
            var cgm = new ConsoleChar { LineChar = LineCharHorizontal, ForegroundColor = Green };
            var cge = new ConsoleChar { LineChar = LineChar.None, ForegroundColor = Green };
            var cbs = new ConsoleChar { LineChar = LineChar.None, ForegroundColor = Blue };
            var cbm = new ConsoleChar { LineChar = LineCharHorizontalDouble, ForegroundColor = Blue };
            var cbe = new ConsoleChar { LineChar = LineCharLeft(LineWidth.Double), ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(crs, crm, crm, crm, cre);
            buffer.GetLine(1).Should().Equal(c0m, cgs, cgm, cge, c0m);
            buffer.GetLine(2).Should().Equal(c0m, c0m, cbs, cbe, c0m);
            buffer.GetLine(3).Should().Equal(cbm, cbm, cbm, cbm, cbm);
        }

        [Fact]
        public void DrawVerticalLineCapFull()
        {
            var buffer = new ConsoleBuffer(4);

            buffer.DrawVerticalLine(0, 0, 5, Red, LineWidth.Single, DrawLineFlags.CapFull);
            buffer.DrawVerticalLine(1, 1, 3, Green, LineWidth.Single, DrawLineFlags.CapFull);
            buffer.DrawVerticalLine(3, -1, 10, Blue, LineWidth.Double, DrawLineFlags.CapFull);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Green };
            var cb = new ConsoleChar { LineChar = LineCharVerticalDouble, ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(cr, c0, c0, cb);
            buffer.GetLine(1).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(2).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(3).Should().Equal(cr, cg, c0, cb);
            buffer.GetLine(4).Should().Equal(cr, c0, c0, cb);
        }

        [Fact]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        public void DrawVerticalLineCapCenter()
        {
            var buffer = new ConsoleBuffer(4);

            buffer.DrawVerticalLine(0, 0, 5, Red, LineWidth.Single, DrawLineFlags.CapCenter);
            buffer.DrawVerticalLine(1, 1, 3, Green, LineWidth.Single, DrawLineFlags.CapStartCenter);
            buffer.DrawVerticalLine(2, 2, 2, Blue, LineWidth.Double, DrawLineFlags.CapEndCenter);
            buffer.DrawVerticalLine(3, -1, 10, Blue, LineWidth.Double, DrawLineFlags.CapCenter);

            var c0m = new ConsoleChar();
            var crs = new ConsoleChar { LineChar = LineCharBottom(LineWidth.Single), ForegroundColor = Red };
            var crm = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Red };
            var cre = new ConsoleChar { LineChar = LineCharTop(LineWidth.Single), ForegroundColor = Red };
            var cgs = new ConsoleChar { LineChar = LineCharBottom(LineWidth.Single), ForegroundColor = Green };
            var cgm = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Green };
            var cge = new ConsoleChar { LineChar = LineChar.None, ForegroundColor = Green };
            var cbs = new ConsoleChar { LineChar = LineChar.None, ForegroundColor = Blue };
            var cbm = new ConsoleChar { LineChar = LineCharVerticalDouble, ForegroundColor = Blue };
            var cbe = new ConsoleChar { LineChar = LineCharTop(LineWidth.Double), ForegroundColor = Blue };

            buffer.GetLine(0).Should().Equal(crs, c0m, c0m, cbm);
            buffer.GetLine(1).Should().Equal(crm, cgs, c0m, cbm);
            buffer.GetLine(2).Should().Equal(crm, cgm, cbs, cbm);
            buffer.GetLine(3).Should().Equal(crm, cge, cbe, cbm);
            buffer.GetLine(4).Should().Equal(cre, c0m, c0m, cbm);
        }

        [Fact]
        public void DrawLine()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawLine(Line.Vertical(1, 0, 3), Red, LineWidth.Single, DrawLineFlags.CapFull);
            buffer.DrawLine(Line.Horizontal(0, 1, 3), Green, LineWidth.Double, DrawLineFlags.CapFull);

            var c0 = new ConsoleChar();
            var cr = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Red };
            var cg = new ConsoleChar { LineChar = LineCharHorizontalDouble, ForegroundColor = Green };
            var cx = new ConsoleChar { LineChar = LineCharHorizontalDoubleVertical, ForegroundColor = Green };

            buffer.GetLine(0).Should().Equal(c0, cr, c0);
            buffer.GetLine(1).Should().Equal(cg, cx, cg);
            buffer.GetLine(2).Should().Equal(c0, cr, c0);
        }

        [Fact]
        public void DrawRectangle()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawRectangle(new Rect(0, 0, 3, 3), Red, new LineThickness(LineWidth.Single, LineWidth.Double));

            var c0m = new ConsoleChar();
            var ctl = new ConsoleChar { LineChar = LineCharBottomRight(LineWidth.Single, LineWidth.Double), ForegroundColor = Red };
            var ctr = new ConsoleChar { LineChar = LineCharBottomLeft(LineWidth.Single, LineWidth.Double), ForegroundColor = Red };
            var cbl = new ConsoleChar { LineChar = LineCharTopRight(LineWidth.Single, LineWidth.Double), ForegroundColor = Red };
            var cbr = new ConsoleChar { LineChar = LineCharTopLeft(LineWidth.Single, LineWidth.Double), ForegroundColor = Red };
            var chm = new ConsoleChar { LineChar = LineCharHorizontalDouble, ForegroundColor = Red };
            var cvm = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Red };

            buffer.GetLine(0).Should().Equal(ctl, chm, ctr);
            buffer.GetLine(1).Should().Equal(cvm, c0m, cvm);
            buffer.GetLine(2).Should().Equal(cbl, chm, cbr);
        }

        [Fact]
        public void DrawRectangleSimple()
        {
            var buffer = new ConsoleBuffer(3);

            buffer.DrawRectangle(new Rect(0, 0, 3, 3), Red, LineWidth.Single);

            var c0m = new ConsoleChar();
            var ctl = new ConsoleChar { LineChar = LineCharBottomRight(LineWidth.Single, LineWidth.Single), ForegroundColor = Red };
            var ctr = new ConsoleChar { LineChar = LineCharBottomLeft(LineWidth.Single, LineWidth.Single), ForegroundColor = Red };
            var cbl = new ConsoleChar { LineChar = LineCharTopRight(LineWidth.Single, LineWidth.Single), ForegroundColor = Red };
            var cbr = new ConsoleChar { LineChar = LineCharTopLeft(LineWidth.Single, LineWidth.Single), ForegroundColor = Red };
            var chm = new ConsoleChar { LineChar = LineCharHorizontal, ForegroundColor = Red };
            var cvm = new ConsoleChar { LineChar = LineCharVertical, ForegroundColor = Red };

            buffer.GetLine(0).Should().Equal(ctl, chm, ctr);
            buffer.GetLine(1).Should().Equal(cvm, c0m, cvm);
            buffer.GetLine(2).Should().Equal(cbl, chm, cbr);
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