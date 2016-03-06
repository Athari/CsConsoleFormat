using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class WrapTests : ElementTestsBase
    {
        [Fact]
        public void NegativeItemWidth()
        {
            var wrap = new Wrap();

            new Action(() => wrap.ItemWidth = -1).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NegativeItemHeight()
        {
            var wrap = new Wrap();

            new Action(() => wrap.ItemHeight = -1).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NoChildren()
        {
            var wrap = new Wrap();

            new Action(() => RenderOn1x1(wrap)).ShouldNotThrow();
        }

        [Fact]
        public void EmptyChild()
        {
            var wrap = new Wrap { Children = { new Div() } };

            new Action(() => RenderOn1x1(wrap)).ShouldNotThrow();
        }

        [Fact]
        public void FixedSizeExact()
        {
            var wrap = new Wrap {
                ItemWidth = 3,
                ItemHeight = 2,
                Children = {
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abcghi",
                "defjkl",
                "mnostu",
                "pqrvwx");
        }

        [Fact]
        public void FixedSizeSmaller()
        {
            var wrap = new Wrap {
                ItemWidth = 3,
                ItemHeight = 2,
                Children = {
                    CreateRectDiv(3, 1),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(1, 1),
                    CreateRectDiv(2, 1),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abcde ",
                "   fg ",
                "h  ij ",
                "      ");
        }

        [Fact]
        public void FixedSizeLarger()
        {
            var wrap = new Wrap {
                ItemWidth = 2,
                ItemHeight = 1,
                Children = {
                    CreateRectDiv(3, 1),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(1, 1),
                    CreateRectDiv(2, 1),
                }
            };

            GetRenderedText(wrap, 5).Should().BeLines(
                "abde ",
                "h ij ");
        }

        [Fact]
        public void AutoSizeFixed()
        {
            var wrap = new Wrap {
                Children = {
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abcghi",
                "defjkl",
                "mnostu",
                "pqrvwx");
        }

        [Fact]
        public void AutoSizeFixedVertical()
        {
            var wrap = new Wrap {
                Orientation = Orientation.Vertical,
                Height = 4,
                Children = {
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                    CreateRectDiv(3, 2),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abcmno",
                "defpqr",
                "ghistu",
                "jklvwx");
        }

        [Fact]
        public void AutoSizeMixed()
        {
            var wrap = new Wrap {
                Children = {
                    CreateRectDiv(3, 1),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(2, 1),
                    CreateRectDiv(1, 1),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abcde ",
                "   fg ",
                "hij   ");
        }

        [Fact]
        public void AutoSizeMixedVertical()
        {
            var wrap = new Wrap {
                Orientation = Orientation.Vertical,
                Height = 3,
                Children = {
                    CreateRectDiv(3, 1),
                    CreateRectDiv(2, 2),
                    CreateRectDiv(2, 1),
                    CreateRectDiv(1, 1),
                }
            };

            GetRenderedText(wrap, 6).Should().BeLines(
                "abchi ",
                "de j  ",
                "fg    ");
        }

        [Fact]
        public void AutoSizeTooLarge()
        {
            var wrap = new Wrap {
                Children = {
                    CreateRectDiv(5, 1),
                    CreateRectDiv(6, 1),
                }
            };

            GetRenderedText(wrap, 4).Should().BeLines(
                "abcd",
                "fghi");
        }
    }
}