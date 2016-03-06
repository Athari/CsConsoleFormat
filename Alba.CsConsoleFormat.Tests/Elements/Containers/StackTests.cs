using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class StackTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren()
        {
            var stack = new Stack();

            new Action(() => RenderOn1x1(stack)).ShouldNotThrow();
        }

        [Fact]
        public void EmptyChild()
        {
            var stack = new Stack { Children = { new Div() } };

            new Action(() => RenderOn1x1(stack)).ShouldNotThrow();
        }

        [Fact]
        public void SizeMixed()
        {
            var stack = new Stack {
                Children = {
                    CreateRectDiv(3, 1),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(3, 1),
                    CreateRectDiv(1, 1),
                    CreateRectDiv(5, 1),
                }
            };

            GetRenderedText(stack, 4).Should().BeLines(
                "abc ",
                "de  ",
                "fg  ",
                "hij ",
                "k   ",
                "lmno");
        }

        [Fact]
        public void SizeMixedHorizontal()
        {
            var stack = new Stack {
                Orientation = Orientation.Horizontal,
                Height = 4,
                Children = {
                    CreateRectDiv(1, 3),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(1, 3),
                    CreateRectDiv(1, 1),
                    CreateRectDiv(1, 5),
                }
            };

            GetRenderedText(stack, 7).Should().BeLines(
                "adehkl ",
                "bfgi m ",
                "c  j n ",
                "     o ");
        }
    }
}