using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class DockTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren ()
        {
            var dock = new Dock();

            new Action(() => RenderOn1x1(dock)).ShouldNotThrow();
        }

        [Fact]
        public void Flower ()
        {
            var dock = new Dock()
                .AddChildren(
                    new Fill { Char = 'a', Width = 4, Margin = new Thickness(1, 1, 0, 1), Padding = 1 }
                        .Set(Dock.ToProperty, DockTo.Left),
                    new Fill { Char = 'b', Height = 3, Margin = new Thickness(1, 1, 1, 0), Padding = 1 }
                        .Set(Dock.ToProperty, DockTo.Top),
                    new Fill { Char = 'c', Width = 2, Margin = new Thickness(0, 1, 1, 1), Padding = 1 }
                        .Set(Dock.ToProperty, DockTo.Right),
                    new Fill { Char = 'd', Height = 1, Margin = new Thickness(1, 0, 1, 1), Padding = 1 }
                        .Set(Dock.ToProperty, DockTo.Bottom),
                    new Fill { Char = 'e', Margin = 1, Padding = 1 }
                );

            GetRenderedText(dock, 12).Should().BeLines(
                "            ",
                " aaaa bbbbb ",
                " aaaa bbbbb ",
                " aaaa bbbbb ",
                " aaaa       ",
                " aaaa ee cc ",
                " aaaa ee cc ",
                " aaaa    cc ",
                " aaaa dd cc ",
                "            ");
        }
    }
}