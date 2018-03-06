using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class BorderTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren()
        {
            var border = new Border();

            new Action(() => RenderOn1x1(border)).Should().NotThrow();
        }

        [Fact]
        public void RenderPadding()
        {
            var border = new Border {
                Padding = new Thickness(2, 1),
                Children = { CreateRectDiv(1, 1) },
            };

            GetRenderedText(border, 6).Should().BeLines(
                "      ",
                "  a   ",
                "      ");
        }

        [Fact]
        public void RenderBorder()
        {
            var border = new Border {
                Padding = new Thickness(2, 0, 1, 0),
                Stroke = new LineThickness(LineWidth.None, LineWidth.Double, LineWidth.Single, LineWidth.Double),
                Children = { CreateRectDiv(2, 1) },
            };

            GetRenderedText(border, 6).Should().BeLines(
                "═════╕",
                "  ab │",
                "═════╛");
        }

        [Fact]
        public void RenderOuterShadow()
        {
            var border = new Border {
                Padding = new Thickness(1, 0, 0, 1),
                Shadow = new Thickness(3, 2, 2, 1),
                Children = { CreateRectDiv(1, 1) },
            };

            GetRenderedText(border, 7).Should().BeLines(
                "▄▄▄▄▄▄▄",
                "███████",
                "███ a██",
                "███  ██",
                "▀▀▀▀▀▀▀");
        }

        [Fact]
        public void RenderBottomRightShadow()
        {
            var border = new Border {
                Shadow = new Thickness(-1, -1, 2, 3),
                Children = { CreateRectDiv(4, 2) },
            };

            GetRenderedText(border, 7).Should().BeLines(
                "abcd ▄▄",
                "efgh ██",
                " ██████",
                " ██████",
                " ▀▀▀▀▀▀");
        }

        [Fact]
        public void RenderAllDecorations()
        {
            var border = new Border {
                Stroke = new LineThickness(LineWidth.Double, LineWidth.Single),
                Shadow = new Thickness(-1, -1, 1, 1),
                ShadowColor = ConsoleColor.DarkYellow,
                Padding = new Thickness(1, 0),
                Background = ConsoleColor.Yellow,
                Children = { CreateRectDiv(3, 1) },
            };

            GetRenderedText(border, 8).Should().BeLines(
                "╓─────╖▄",
                "║ abc ║█",
                "╙─────╜█",
                " ▀▀▀▀▀▀▀");
        }
    }
}