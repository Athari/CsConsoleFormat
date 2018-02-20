using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class ConsoleRenderTargetTests
    {
        [Fact]
        public void Render()
        {
            var buffer = new ConsoleBuffer(3);
            var target = new ConsoleRenderTarget();

            TextWriter oldOut = Console.Out;
            try {
                var stringOut = new StringWriter();
                Console.SetOut(stringOut);

                buffer.DrawString(0, 0, ConsoleColor.White, "Foo");
                target.Render(buffer);

                stringOut.ToString().Should().Be($"Foo{Environment.NewLine}");
            }
            finally {
                Console.SetOut(oldOut);
            }
        }
    }
}