using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class AbsoluteTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren()
        {
            var absolute = new Absolute();

            new Action(() => RenderOn1x1(absolute)).Should().NotThrow();
        }

        [Fact]
        public void RenderPyramid()
        {
            var absolute = new Absolute {
                Width = 11, Height = 7,
                Children = {
                    new Fill {
                        Char = 'a', Width = 4, Height = 2,
                        [Absolute.LeftProperty] = 1, [Absolute.TopProperty] = 1,
                    },
                    new Fill {
                        Char = 'b', Width = 4, Height = 2,
                        [Absolute.LeftProperty] = 1, [Absolute.BottomProperty] = 1,
                    },
                    new Fill {
                        Char = 'c', Width = 4, Height = 2,
                        [Absolute.RightProperty] = 1, [Absolute.TopProperty] = 1,
                    },
                    new Fill {
                        Char = 'd', Width = 4, Height = 2,
                        [Absolute.RightProperty] = 1, [Absolute.BottomProperty] = 1,
                    },
                    new Fill {
                        Char = 'e', Width = 5, Height = 3,
                        [Absolute.LeftProperty] = 3, [Absolute.TopProperty] = 2,
                    },
                }
            };

            GetRenderedText(absolute, 11).Should().BeLines(
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