using System;
using System.Collections.Generic;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class ElementTests
    {
        [Fact]
        public void CloneSimple()
        {
            var el = new MyElement {
                Name = "Foo",
                Color = ConsoleColor.Red,
                Background = ConsoleColor.Blue,
                Visibility = Visibility.Hidden,
                Language = new XmlLanguage("en-us"),
            };

            var clone = (Element)el.Clone();

            clone.Should().NotBeSameAs(el);
            clone.Name.Should().Be(el.Name);
            clone.Color.Should().Be(el.Color);
            clone.Background.Should().Be(el.Background);
            clone.Visibility.Should().Be(el.Visibility);
            clone.Language.Should().Be(el.Language);
        }

        [Fact]
        public void CloneChildren()
        {
            var el = new MyElement {
                Children = {
                    new Span("Foo"),
                    new Span("Bar"),
                }
            };

            var clone = (Element)el.Clone();

            clone.Should().NotBeSameAs(el);
            clone.Children[0].Should().NotBeSameAs(el.Children[0]);
            clone.Children[0].As<Span>().Text.Should().Be("Foo");
            clone.Children[1].Should().NotBeSameAs(el.Children[1]);
            clone.Children[1].As<Span>().Text.Should().Be("Bar");
        }

        [Fact]
        public void GenerateVisualTree_Empty()
        {
            var el = new MyElement();

            el.GenerateVisualTree();

            el.VisualChildren.Should().BeEmpty();
        }

        [Fact]
        public void GenerateVisualTree_InlinesAreGroupedIntoInlinesContainer()
        {
            var el = new MyElement {
                Children = {
                    new Span("Foo"),
                    new Span("Bar"),
                }
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().ContainSingle();
            el.VisualChildren[0].Should().BeOfType<InlineContainer>();
            {
                IList<Element> children0 = el.VisualChildren[0].As<InlineContainer>().VisualChildren;
                children0.Should().HaveCount(2);

                children0[0].As<Span>().Text.Should().Be("Foo");
                children0[1].As<Span>().Text.Should().Be("Bar");
            }
        }

        [Fact]
        public void GenerateVisualTree_NesteddInlinesAreGroupedIntoInlinesContainer()
        {
            var el = new MyElement {
                Children = {
                    new Span {
                        Name = "Foo",
                        Children = {
                            new Span { Name = "Bar" },
                        },
                    },
                },
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().ContainSingle();
            el.VisualChildren[0].Should().BeOfType<InlineContainer>();
            {
                IList<Element> children0 = el.VisualChildren[0].As<InlineContainer>().VisualChildren;
                children0.Should().HaveCount(1);
                children0[0].As<Span>().Name.Should().Be("Foo");
                {
                    IList<Element> children00 = children0[0].As<Span>().VisualChildren;
                    children00.Should().HaveCount(1);
                    children00[0].As<Span>().Name.Should().Be("Bar");
                }
            }
        }

        [Fact]
        public void GenerateVisualTree_InlinesSeparatedByBlockAreGroupedIntoSeparateInlinesContainers()
        {
            var el = new MyElement {
                Children = {
                    new Span("1"),
                    new Span("2"),
                    new Border(),
                    new Span("3"),
                    new Span("4"),
                }
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().HaveCount(1);
            el.VisualChildren[0].Should().BeOfType<Stack>();
            {
                IList<Element> children0 = el.VisualChildren[0].As<Stack>().VisualChildren;
                children0.Should().HaveCount(3);

                children0[0].Should().BeOfType<InlineContainer>();
                {
                    IList<Element> children01 = children0[0].As<InlineContainer>().VisualChildren;
                    children01.Should().HaveCount(2);
                    children01[0].As<Span>().Text.Should().Be("1");
                    children01[1].As<Span>().Text.Should().Be("2");
                }
                children0[1].Should().BeOfType<Border>();
                children0[2].Should().BeOfType<InlineContainer>();
                {
                    IList<Element> children02 = children0[2].As<InlineContainer>().VisualChildren;
                    children02.Should().HaveCount(2);
                    children02[0].As<Span>().Text.Should().Be("3");
                    children02[1].As<Span>().Text.Should().Be("4");
                }
            }
        }

        [Fact]
        public void GenerateVisualTree_BlockAddedAsIs()
        {
            var el = new MyElement {
                Children = {
                    new Border(),
                }
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().HaveCount(1);
            el.VisualChildren[0].Should().BeOfType<Border>();
        }

        [Fact]
        public void GenerateVisualTree_BlocksAreGroupedIntoStack()
        {
            var el = new MyElement {
                Children = {
                    new Div { Name = "1" },
                    new Div { Name = "2" },
                }
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().HaveCount(1);
            el.VisualChildren[0].Should().BeOfType<Stack>();
            {
                IList<Element> children0 = el.VisualChildren[0].As<Stack>().VisualChildren;
                children0.Should().HaveCount(2);

                children0[0].As<Div>().Name.Should().Be("1");
                children0[1].As<Div>().Name.Should().Be("2");
            }
        }

        [Fact]
        public void GenerateVisualTree_BlocksInContainerAreAddedAsIs()
        {
            var el = new MyElement {
                Children = {
                    new Wrap {
                        Children = {
                            new Div { Name = "1" },
                            new Div { Name = "2" },
                        }
                    }
                }
            };

            el.GenerateVisualTree();

            el.VisualChildren.Should().HaveCount(1);
            el.VisualChildren[0].Should().BeOfType<Wrap>();
            {
                IList<Element> children0 = el.VisualChildren[0].As<Wrap>().VisualChildren;
                children0.Should().HaveCount(2);

                children0[0].As<Div>().Name.Should().Be("1");
                children0[1].As<Div>().Name.Should().Be("2");
            }
        }

        private class MyElement : BlockElement
        {}
    }
}