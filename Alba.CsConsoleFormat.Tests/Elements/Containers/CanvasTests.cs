using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class CanvasTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren()
        {
            var canvas = new Canvas();

            new Action(() => RenderOn1x1(canvas)).Should().NotThrow();
        }

        [Fact]
        public void RenderPyramid()
        {
            var canvas = new Canvas {
                Width = 11, Height = 7,
                Children = {
                    new Fill {
                        Char = 'a', Width = 4, Height = 2,
                        [Canvas.LeftProperty] = 1, [Canvas.TopProperty] = 1,
                    },
                    new Fill {
                        Char = 'b', Width = 4, Height = 2,
                        [Canvas.LeftProperty] = 1, [Canvas.BottomProperty] = 1,
                    },
                    new Fill {
                        Char = 'c', Width = 4, Height = 2,
                        [Canvas.RightProperty] = 1, [Canvas.TopProperty] = 1,
                    },
                    new Fill {
                        Char = 'd', Width = 4, Height = 2,
                        [Canvas.RightProperty] = 1, [Canvas.BottomProperty] = 1,
                    },
                    new Fill {
                        Char = 'e', Width = 5, Height = 3,
                        [Canvas.LeftProperty] = 3, [Canvas.TopProperty] = 2,
                    },
                }
            };

            GetRenderedText(canvas, 11).Should().BeLines(
                "           ",
                " aaaa cccc ",
                " aaeeeeecc ",
                "   eeeee   ",
                " bbeeeeedd ",
                " bbbb dddd ",
                "           ");
        }
    }
}