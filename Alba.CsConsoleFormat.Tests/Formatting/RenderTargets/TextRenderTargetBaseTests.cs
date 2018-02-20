using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class TextRenderTargetBaseTests
    {
        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        [SuppressMessage("ReSharper", "LocalNameCapturedOnly")]
        public void NullArguments()
        {
            var output = Stream.Null;
            var buffer = new ConsoleBuffer(80);

            new Action(() => new RenderTarget((Stream)null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(output));
            new Action(() => new RenderTarget(output).Render(null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(buffer));
        }

        [Fact]
        public void UseAfterDispose()
        {
            void AfterDispose(Action<RenderTarget> use)
            {
                var target = new RenderTarget(Stream.Null);
                target.Dispose();
                new Action(() => use(target)).ShouldThrow<ObjectDisposedException>();
            }

            var buffer = new ConsoleBuffer(80);

            AfterDispose(t => t.Render(buffer));
            AfterDispose(t => t.OutputText.As<string>());
        }

        [Fact]
        public void DoubleDispose()
        {
            var target = new RenderTarget(Stream.Null);
            target.Dispose();

            new Action(() => target.Dispose()).ShouldNotThrow<Exception>();
        }

        [Fact]
        public void StreamEncodingASCII()
        {
            var stream = new MemoryStream(3);
            var buffer = new ConsoleBuffer(80);

            using (var target = new RenderTarget(stream, Encoding.ASCII))
                target.Render(buffer);

            stream.GetBuffer().Should().Equal((byte)'F', (byte)'o', (byte)'o');
        }

        [Fact]
        public void StreamEncodingDefaultUnicode()
        {
            var stream = new MemoryStream(6);
            var buffer = new ConsoleBuffer(80);

            using (var target = new RenderTarget(stream))
                target.Render(buffer);

            stream.GetBuffer().Should().Equal((byte)'F', 0, (byte)'o', 0, (byte)'o', 0);
        }

        [Fact]
        public void StreamLeaveOpen()
        {
            var stream = new MemoryStream(3);
            var buffer = new ConsoleBuffer(80);

            using (var target = new RenderTarget(stream, Encoding.ASCII, true))
                target.Render(buffer);

            stream.GetBuffer().Should().Equal((byte)'F', (byte)'o', (byte)'o');
        }

        [Fact]
        public void OutputTextWithoutStringWriter()
        {
            var target = new RenderTarget(TextWriter.Null);

            new Action(() => target.OutputText.As<string>()).ShouldThrow<InvalidOperationException>().WithMessage($"*{nameof(StringWriter)}*");
        }

        private sealed class RenderTarget : TextRenderTargetBase
        {
            public RenderTarget([NotNull] Stream output, [CanBeNull] Encoding encoding = null, bool leaveOpen = false) : base(output, encoding, leaveOpen)
            { }

            public RenderTarget([CanBeNull] TextWriter writer = null) : base(writer)
            { }

            protected override void RenderOverride(IConsoleBufferSource buffer)
            {
                ThrowIfDisposed();
                Debug.Assert(Writer != null);
                Writer.Write("Foo");
            }
        }
    }
}