using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Alba.CsConsoleFormat.Tests.Resources;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class ConsoleRendererTests : ElementTestsBase
    {
        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
        [SuppressMessage("ReSharper", "LocalNameCapturedOnly")]
        public void NullArguments()
        {
            var type = GetType();
            var assembly = type.GetAssembly();
            var stream = Stream.Null;
            var dataContext = new object();
            var document = new Document();
            var target = new TextRenderTarget();
            var buffer = new ConsoleBuffer(80);
            var renderRect = Rect.Empty;

          #if XAML
            var resourceName = "";

            new Action(() => ConsoleRenderer.ReadElementFromStream<Br>(null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(stream));
            new Action(() => ConsoleRenderer.ReadElementFromResource<Br>((Assembly)null, resourceName, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(assembly));
            new Action(() => ConsoleRenderer.ReadElementFromResource<Br>(assembly, null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(resourceName));
            new Action(() => ConsoleRenderer.ReadElementFromResource<Br>((Type)null, resourceName, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(type));
            new Action(() => ConsoleRenderer.ReadElementFromResource<Br>(type, null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(resourceName));
            new Action(() => ConsoleRenderer.ReadDocumentFromStream(null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(stream));
            new Action(() => ConsoleRenderer.ReadDocumentFromResource((Assembly)null, resourceName, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(assembly));
            new Action(() => ConsoleRenderer.ReadDocumentFromResource(assembly, null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(resourceName));
            new Action(() => ConsoleRenderer.ReadDocumentFromResource((Type)null, resourceName, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(type));
            new Action(() => ConsoleRenderer.ReadDocumentFromResource(type, null, dataContext))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(resourceName));
          #endif
            new Action(() => ConsoleRenderer.RenderDocument(null))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(document));
            new Action(() => ConsoleRenderer.RenderDocumentToBuffer(null, buffer, renderRect))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(document));
            new Action(() => ConsoleRenderer.RenderDocumentToBuffer(document, null, renderRect))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(buffer));
            new Action(() => ConsoleRenderer.RenderDocumentToText(null, target))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(document));
            new Action(() => ConsoleRenderer.RenderDocumentToText(document, null))
                .Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(target));
        }

        #if XAML
        [Fact]
        public void ReadElementFromStream()
        {
            Span span = ConsoleRenderer.ReadElementFromStream<Span>(StringToStream($"<Span {XamlNS} Text='Foo'/>"), null);
            span.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadElementFromStreamWithContext()
        {
            Span span = ConsoleRenderer.ReadElementFromStream<Span>(StringToStream($"<Span {XamlNS} Text='{{Get Length}}'/>"), "abc");
            span.Text.Should().Be("3");
        }

        [Fact]
        public void ReadElementFromResource()
        {
            Span span = ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res).GetAssembly(), $"{typeof(Res).Namespace}.{Res.Span}", null);
            span.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadElementFromResourceError()
        {
            new Action(() => ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res).GetAssembly(), "Oops", null))
                .Should().Throw<FileNotFoundException>().WithMessage("*Oops*not found*");
        }

        [Fact]
        public void ReadElementFromResourceWithContext()
        {
            Span span = ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res).GetAssembly(), $"{typeof(Res).Namespace}.{Res.SpanWithContext}", "abc");
            span.Text.Should().Be("3");
        }

        [Fact]
        public void ReadElementFromResourceByType()
        {
            Span span = ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res), Res.Span, null);
            span.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadElementFromResourceByTypeError()
        {
            new Action(() => ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res), "Oops", null))
                .Should().Throw<FileNotFoundException>().WithMessage("*Oops*not found*");
        }

        [Fact]
        public void ReadElementFromResourceByTypeWithContext()
        {
            Span span = ConsoleRenderer.ReadElementFromResource<Span>(typeof(Res), Res.SpanWithContext, "abc");
            span.Text.Should().Be("3");
        }

        [Fact]
        public void ReadDocumentFromStream()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromStream(StringToStream($"<Document {XamlNS}><Span Text='Foo'/></Document>"), null);
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadDocumentFromStreamWithContext()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromStream(StringToStream($"<Document {XamlNS}><Span Text='{{Get Length}}'/></Document>"), "abc");
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("3");
        }

        [Fact]
        public void ReadDocumentFromResource()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromResource(typeof(Res).GetAssembly(), $"{typeof(Res).Namespace}.{Res.DocumentSpan}", null);
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadDocumentFromResourceWithContext()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromResource(typeof(Res).GetAssembly(), $"{typeof(Res).Namespace}.{Res.DocumentSpanWithContext}", "abc");
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("3");
        }

        [Fact]
        public void ReadDocumentFromResourceByType()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromResource(typeof(Res), Res.DocumentSpan, null);
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("Foo");
        }

        [Fact]
        public void ReadDocumentFromResourceByTypeWithContext()
        {
            Document doc = ConsoleRenderer.ReadDocumentFromResource(typeof(Res), Res.DocumentSpanWithContext, "abc");
            doc.Children.Should().ContainSingle().Which.Should().BeOfType<Span>().Which.Text.Should().Be("3");
        }

        #endif

        [Fact]
        public void RenderDocumentToString()
        {
            var doc = new Document { Children = { new Span("Foo") } };
            var target = new TextRenderTarget();
            ConsoleRenderer.RenderDocument(doc, target, new Rect(0, 0, 3, 1));

            target.OutputText.Should().Be($"Foo{Environment.NewLine}");
        }

        private static Stream StringToStream([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var stream = new MemoryStream(value.Length);
            var writer = new StreamWriter(stream) { AutoFlush = true };
            writer.Write(value);
            stream.Position = 0;
            return stream;
        }
    }
}