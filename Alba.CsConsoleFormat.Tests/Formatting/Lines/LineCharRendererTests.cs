using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class LineCharRendererTests : ElementTestsBase
    {
        [Fact]
        public void CharLineCharRendererEmpty()
        {
            var none = LineChar.None;
            LineCharRenderer.Char('*').GetChar(none, none, none, none, none).Should().Be('\0');
        }

        [Fact]
        public void CharLineCharRendererSimple()
        {
            var buffer = new ConsoleBuffer(6) { LineCharRenderer = LineCharRenderer.Char('*') };

            buffer.DrawHorizontalLine(0, 1, 6, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(1, 3, 4);
            buffer.DrawVerticalLine(2, 0, 5);
            buffer.DrawVerticalLine(4, 1, 3, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "  *   ",
                "******",
                "  * * ",
                " **** ",
                "  *   ");
        }

        [Fact]
        public void SimpleLineCharRendererEmpty()
        {
            var none = LineChar.None;
            LineCharRenderer.Simple.GetChar(none, none, none, none, none).Should().Be('\0');
        }

        [Fact]
        public void SimpleLineCharRendererSimple()
        {
            var buffer = new ConsoleBuffer(6) { LineCharRenderer = LineCharRenderer.Simple };

            buffer.DrawHorizontalLine(0, 1, 6, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(1, 3, 4);
            buffer.DrawVerticalLine(2, 0, 5);
            buffer.DrawVerticalLine(4, 1, 3, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "  |   ",
                "==+=+=",
                "  | | ",
                " -+-+ ",
                "  |   ");
        }

        [Fact]
        public void BoxLineCharRendererEmpty()
        {
            var none = LineChar.None;
            LineCharRenderer.Box.GetChar(none, none, none, none, none).Should().Be('\0');
        }

        [Fact]
        public void BoxLineCharRendererSimple()
        {
            var buffer = new ConsoleBuffer(6) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 1, 6, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(1, 3, 4);
            buffer.DrawVerticalLine(2, 0, 5);
            buffer.DrawVerticalLine(4, 1, 3, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "  │   ",
                "══╪═╦═",
                "  │ ║ ",
                " ─┼─╜ ",
                "  │   ");
        }

        [Fact]
        public void BoxLineCharRendererBox2x2Single()
        {
            var buffer = new ConsoleBuffer(5) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 5);
            buffer.DrawHorizontalLine(0, 2, 5);
            buffer.DrawHorizontalLine(0, 4, 5);
            buffer.DrawVerticalLine(0, 0, 5);
            buffer.DrawVerticalLine(2, 0, 5);
            buffer.DrawVerticalLine(4, 0, 5);

            GetRenderedText(buffer).Should().BeLines(
                "┌─┬─┐",
                "│ │ │",
                "├─┼─┤",
                "│ │ │",
                "└─┴─┘");
        }

        [Fact]
        public void BoxLineCharRendererBox2x2Wide()
        {
            var buffer = new ConsoleBuffer(5) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 5, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(0, 2, 5, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(0, 4, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 0, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(2, 0, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(4, 0, 5, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "╔═╦═╗",
                "║ ║ ║",
                "╠═╬═╣",
                "║ ║ ║",
                "╚═╩═╝");
        }

        [Fact]
        public void BoxLineCharRendererBox2x2Mixed()
        {
            var buffer = new ConsoleBuffer(5) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 5, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(0, 2, 5);
            buffer.DrawHorizontalLine(0, 4, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 0, 5);
            buffer.DrawVerticalLine(2, 0, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(4, 0, 5);

            GetRenderedText(buffer).Should().BeLines(
                "╒═╦═╕",
                "│ ║ │",
                "├─╫─┤",
                "│ ║ │",
                "╘═╩═╛");
        }

        [Fact]
        public void BoxLineCharRendererBox2x2MixedAlt()
        {
            var buffer = new ConsoleBuffer(5) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 5);
            buffer.DrawHorizontalLine(0, 2, 5, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(0, 4, 5);
            buffer.DrawVerticalLine(0, 0, 5, null, LineWidth.Wide);
            buffer.DrawVerticalLine(2, 0, 5);
            buffer.DrawVerticalLine(4, 0, 5, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "╓─┬─╖",
                "║ │ ║",
                "╠═╪═╣",
                "║ │ ║",
                "╙─┴─╜");
        }

        [Fact]
        public void BoxLineCharRendererLines()
        {
            var buffer = new ConsoleBuffer(3) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 3);
            buffer.DrawHorizontalLine(0, 2, 3, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 0, 1);
            buffer.DrawVerticalLine(2, 0, 1, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 2, 1);
            buffer.DrawVerticalLine(2, 2, 1, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "┼─╫",
                "   ",
                "╪═╬");
        }

        [Fact]
        public void BoxLineCharRendererPixels()
        {
            var buffer = new ConsoleBuffer(3) { LineCharRenderer = LineCharRenderer.Box };

            buffer.DrawHorizontalLine(0, 0, 1);
            buffer.DrawHorizontalLine(2, 0, 1);
            buffer.DrawHorizontalLine(0, 2, 1, null, LineWidth.Wide);
            buffer.DrawHorizontalLine(2, 2, 1, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 0, 1);
            buffer.DrawVerticalLine(2, 0, 1, null, LineWidth.Wide);
            buffer.DrawVerticalLine(0, 2, 1);
            buffer.DrawVerticalLine(2, 2, 1, null, LineWidth.Wide);

            GetRenderedText(buffer).Should().BeLines(
                "┼ ╫",
                "   ",
                "╪ ╬");
        }
    }
}