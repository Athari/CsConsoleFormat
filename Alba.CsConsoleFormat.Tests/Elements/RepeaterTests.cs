using System;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class RepeaterTests : ElementTestsBase
    {
        [Fact]
        public void NoChildren()
        {
            var repeater = new Repeater();
            var doc = new Document().AddChildren(repeater);

            new Action(() => RenderOn1x1(doc)).Should().NotThrow();
        }

        [Fact]
        public void RepeatTextStatic()
        {
            var repeater = new Repeater {
                Items = new object[] { 1, 1, 1 },
                ItemTemplate = {
                    new Span("a")
                }
            };

            GetRenderedText(repeater, 4).Should().BeLines(
                "aaa ");
        }

        [Fact]
        public void RepeatTextBound()
        {
            var repeater = new Repeater {
                Items = new object[] { 1, 2, 3 },
                ItemTemplate = {
                    new Span().Bind(o => o.Text, (int i) => $"{i}")
                }
            };

            GetRenderedText(repeater, 4).Should().BeLines(
                "123 ");
        }

        [Fact]
        public void RepeatTextBoundNested()
        {
            var repeater = new Repeater {
                Items = new object[] {
                    new object[] { 1, 2 },
                    new object[] { 5, 4, 3 },
                },
                ItemTemplate = {
                    new Div()
                        .AddChildren(
                            new Span().Bind(o => o.Text, (object[] items) => $"{items.Length}: "),
                            new Repeater {
                                ItemTemplate = {
                                    new Span().Bind(o => o.Text, (int i) => $"#{i}"),
                                    new Span(", "),
                                }
                            }.Bind(o => o.Items, (object[] items) => items)
                        )
                }
            };

            GetRenderedText(repeater, 11).Should().BeLines(
                "2: #1, #2, ",
                "3: #5, #4, ",
                "#3,        ");
        }
    }
}