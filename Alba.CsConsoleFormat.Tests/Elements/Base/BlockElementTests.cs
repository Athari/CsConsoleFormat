using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class BlockElementTests : ElementTestsBase
    {
        [Fact]
        public void AlignVisibilityCollapsed()
        {
            var fill = new Fill {
                Char = '_', Background = ConsoleColor.Black, Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 1,
                        AlphaHeight = 1,
                        Visibility = Visibility.Collapsed,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "_____");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeft()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "abc__",
                "def__");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Margin = 1,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "_______",
                "_abc___",
                "_def___",
                "_______");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMinLessThanSize()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MinWidth = 2,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "abc____",
                "def____");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMinLessThanSizeWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MinWidth = 2,
                        Margin = 1,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 9).Should().BeLines(
                "_________",
                "_abc_____",
                "_def_____",
                "_________");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMinMoreThanSize()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MinWidth = 5,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "abc--__",
                "def--__");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMinMoreThanSizeWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MinWidth = 5,
                        Margin = 1,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 9).Should().BeLines(
                "_________",
                "_abc--___",
                "_def--___",
                "_________");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMaxLessThanSize()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MaxWidth = 2,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "ab_____",
                "de_____");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMaxLessThanSizeWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MaxWidth = 4,
                        Margin = 1,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 9).Should().BeLines(
                "_________",
                "_ab______",
                "_de______",
                "_________");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMaxMoreThanSize()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MaxWidth = 5,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "abc____",
                "def____");
        }

        [Fact]
        public void AlignHorizontalSmallBlockLeftMaxMoreThanSizeWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        MaxWidth = 5,
                        Margin = 1,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 9).Should().BeLines(
                "_________",
                "_abc_____",
                "_def_____",
                "_________");
        }

        [Fact]
        public void AlignHorizontalSmallBlockCenter()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Center,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "_abc_",
                "_def_");
        }

        [Fact]
        public void AlignHorizontalSmallBlockCenterRoundLeft()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 2,
                        Align = Align.Center,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "_ab__",
                "_cd__");
        }

        [Fact]
        public void AlignHorizontalSmallBlockRight()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Right,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "__abc",
                "__def");
        }

        [Fact]
        public void AlignHorizontalSmallBlockStretch()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "abc--",
                "def--");
        }

        [Fact]
        public void AlignHorizontalSmallBlockStretchWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Margin = new Thickness(1, 2),
                        Align = Align.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 7).Should().BeLines(
                "_______",
                "_______",
                "_abc--_",
                "_def--_",
                "_______",
                "_______");
        }

        [Fact]
        public void AlignHorizontalLargeBlockLeft()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 1).Should().BeLines(
                "a",
                "d");
        }

        [Fact]
        public void AlignHorizontalLargeBlockLeftWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Margin = new Thickness(2, 1),
                        Align = Align.Left,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "_____",
                "__a__",
                "__d__",
                "_____");
        }

        [Fact]
        public void AlignHorizontalLargeBlockCenter()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Center,
                    }
                }
            };

            GetRenderedText(fill, 1).Should().BeLines(
                "b",
                "e");
        }

        [Fact]
        public void AlignHorizontalLargeBlockCenterRoundLeft()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 2,
                        Align = Align.Center,
                    }
                }
            };

            GetRenderedText(fill, 1).Should().BeLines(
                "a",
                "c");
        }

        [Fact]
        public void AlignHorizontalLargeBlockRight()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Right,
                    }
                }
            };

            GetRenderedText(fill, 1).Should().BeLines(
                "c",
                "f");
        }

        [Fact]
        public void AlignHorizontalLargeBlockStretch()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Align = Align.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 1).Should().BeLines(
                "a",
                "d");
        }

        [Fact]
        public void AlignHorizontalLargeBlockStretchWithMargin()
        {
            var fill = new Fill {
                Char = '_',
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 3,
                        AlphaHeight = 2,
                        Margin = new Thickness(1, 2, 3, 4),
                        Align = Align.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 5).Should().BeLines(
                "_____",
                "_____",
                "_a___",
                "_d___",
                "_____",
                "_____",
                "_____",
                "_____");
        }

        [Fact]
        public void AlignVerticalSmallBlockTop()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Top,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ab",
                "cd",
                "ef",
                "__",
                "__");
        }

        [Fact]
        public void AlignVerticalSmallBlockTopWithMargin()
        {
            var fill = new Fill {
                Char = '_', Height = 7,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        Margin = new Thickness(0, 1),
                        VerticalAlign = VerticalAlign.Top,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "__",
                "ab",
                "cd",
                "ef",
                "__",
                "__",
                "__");
        }

        [Fact]
        public void AlignVerticalSmallBlockCenter()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Center,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "__",
                "ab",
                "cd",
                "ef",
                "__");
        }

        [Fact]
        public void AlignVerticalSmallBlockCenterRoundTop()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 2,
                        VerticalAlign = VerticalAlign.Center,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "__",
                "ab",
                "cd",
                "__",
                "__");
        }

        [Fact]
        public void AlignVerticalSmallBlockBottom()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Bottom,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "__",
                "__",
                "ab",
                "cd",
                "ef");
        }

        [Fact]
        public void AlignVerticalSmallBlockStretch()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ab",
                "cd",
                "ef",
                "--",
                "--");
        }

        [Fact]
        public void AlignVerticalSmallBlockStretchWithMargin()
        {
            var fill = new Fill {
                Char = '_', Height = 5,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        Margin = new Thickness(1, 0),
                        VerticalAlign = VerticalAlign.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 4).Should().BeLines(
                "_ab_",
                "_cd_",
                "_ef_",
                "_--_",
                "_--_");
        }

        [Fact]
        public void AlignVerticalLargeBlockTop()
        {
            var fill = new Fill {
                Char = '_', Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Top,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ab");
        }

        [Fact]
        public void AlignVerticalLargeBlockTopWithMargin()
        {
            var fill = new Fill {
                Char = '_', Height = 2,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        Margin = new Thickness(1, 0, 0, 1),
                        VerticalAlign = VerticalAlign.Top,
                    }
                }
            };

            GetRenderedText(fill, 3).Should().BeLines(
                "_ab",
                "___");
        }

        [Fact]
        public void AlignVerticalLargeBlockCenter()
        {
            var fill = new Fill {
                Char = '_', Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Center,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "cd");
        }

        [Fact]
        public void AlignVerticalLargeBlockCenterRoundTop()
        {
            var fill = new Fill {
                Char = '_', Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 2,
                        VerticalAlign = VerticalAlign.Center,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ab");
        }

        [Fact]
        public void AlignVerticalLargeBlockBottom()
        {
            var fill = new Fill {
                Char = '_', Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Bottom,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ef");
        }

        [Fact]
        public void AlignVerticalLargeBlockStretch()
        {
            var fill = new Fill {
                Char = '_', Height = 1,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        VerticalAlign = VerticalAlign.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 2).Should().BeLines(
                "ab");
        }

        [Fact]
        public void AlignVerticalLargeBlockStretchWithMargin()
        {
            var fill = new Fill {
                Char = '_', Height = 2,
                Children = {
                    new FillAlphabet {
                        AlphaWidth = 2,
                        AlphaHeight = 3,
                        Margin = new Thickness(0, 1, 1, 0),
                        VerticalAlign = VerticalAlign.Stretch,
                    }
                }
            };

            GetRenderedText(fill, 3).Should().BeLines(
                "___",
                "ab_");
        }

        [Fact]
        public void InfiniteDesiredWidth()
        {
            var el = new InfiniteDesiredSize { RawDesiredSize = new Size(Size.Infinity, 1) };
            new Action(() => RenderOn1x1(el)).Should().Throw<InvalidOperationException>().Which.Message.Should().Match("*finite*");
        }

        [Fact]
        public void InfiniteDesiredHeight()
        {
            var el = new InfiniteDesiredSize { RawDesiredSize = new Size(1, Size.Infinity) };
            new Action(() => RenderOn1x1(el)).Should().Throw<InvalidOperationException>().Which.Message.Should().Match("*finite*");
        }

        [Fact]
        public void InfiniteDesiredWidthAndHeight()
        {
            var el = new InfiniteDesiredSize { RawDesiredSize = new Size(Size.Infinity, Size.Infinity) };
            new Action(() => RenderOn1x1(el)).Should().Throw<InvalidOperationException>().Which.Message.Should().Match("*finite*");
        }

        private sealed class InfiniteDesiredSize : BlockElement
        {
            public Size RawDesiredSize { get; set; }

            protected override Size MeasureOverride(Size availableSize) => RawDesiredSize;
        }
    }
}