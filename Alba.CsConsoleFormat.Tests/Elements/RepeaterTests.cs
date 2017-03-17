using System;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class RepeaterTests : ElementTestsBase
    {
        private static PropertyInfo SpanTextProperty => typeof(Span).GetProperty(nameof(Span.Text));
        private static PropertyInfo RepeaterItemsProperty => typeof(Repeater).GetProperty(nameof(Repeater.Items));

        [Fact]
        public void NoChildren()
        {
            var repeater = new Repeater();
            var doc = new Document().AddChildren(repeater);

            new Action(() => RenderOn1x1(doc)).ShouldNotThrow();
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
                    new Span().Name(out Span span)
                }
            };
            span.Bind(SpanTextProperty, new GetExpression());

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
                            new Span().Name(out Span spanHeader),
                            new Repeater {
                                ItemTemplate = {
                                    new Span().Name(out Span spanItem),
                                    new Span(", "),
                                }
                            }.Name(out Repeater innerRepeater)
                        )
                }
            };
            innerRepeater.Bind(RepeaterItemsProperty, new GetExpression());
            spanHeader.Bind(SpanTextProperty, new GetExpression { Path = nameof(Array.Length), Format = "{0}: " });
            spanItem.Bind(SpanTextProperty, new GetExpression { Format = "#{0}" });

            GetRenderedText(repeater, 11).Should().BeLines(
                "2: #1, #2, ",
                "3: #5, #4, ",
                "#3,        ");
        }
    }
}