using System;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class AnsiRenderTargetTests
    {
        [Fact]
        public void Render ()
        {
            var buffer = new ConsoleBuffer(4);
            var target = new AnsiRenderTarget();

            buffer.DrawString(0, 0, ConsoleColor.Red, "r");
            buffer.DrawString(1, 0, ConsoleColor.Green, "g");
            buffer.DrawString(2, 0, ConsoleColor.Blue, "b&");
            target.Render(buffer);

            string Esc = "\x1B[", NewLine = Environment.NewLine, Reset = "0", Red = "91;40", Green = "92;40", Blue = "94;40";
            target.OutputText.Should().Be($"{Esc}{Reset}m{Esc}{Red}mr{Esc}{Green}mg{Esc}{Blue}mb&{NewLine}");
        }

        [Fact]
        public void RenderOverrideColors ()
        {
            var buffer = new ConsoleBuffer(4);
            var target = new AnsiRenderTarget {
                BackgroundOverride = ConsoleColor.White,
                ColorOverride = ConsoleColor.Black,
            };

            buffer.DrawString(0, 0, ConsoleColor.Red, "r");
            buffer.DrawString(1, 0, ConsoleColor.Green, "g");
            buffer.DrawString(2, 0, ConsoleColor.Blue, "b&");
            target.Render(buffer);

            string Esc = "\x1B[", NewLine = Environment.NewLine, Reset = "0", Black = "30;107";
            target.OutputText.Should().Be($"{Esc}{Reset}m{Esc}{Black}mrgb&{NewLine}");
        }
    }
}