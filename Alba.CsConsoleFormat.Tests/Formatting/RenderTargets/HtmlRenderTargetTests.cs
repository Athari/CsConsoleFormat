using System;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class HtmlRenderTargetTests
    {
        [Fact]
        public void Render ()
        {
            var buffer = new ConsoleBuffer(4);
            var target = new HtmlRenderTarget {
                PageTitle = "Hello world & you!",
                Font = "10pt monospace",
            };

            buffer.DrawString(0, 0, ConsoleColor.Red, "r");
            buffer.DrawString(1, 0, ConsoleColor.Green, "g");
            buffer.DrawString(2, 0, ConsoleColor.Blue, "b&");
            target.Render(buffer);

            target.OutputText.Should().Contain("<meta charset=\"utf-16\">");
            target.OutputText.Should().Contain("<title>Hello world &amp; you!</title>");
            target.OutputText.Should().Contain("font: 10pt monospace");
            target.OutputText.Should().Contain("<span style=\"color:#F00;background:#000\">r</span>");
            target.OutputText.Should().Contain("<span style=\"color:#0F0;background:#000\">g</span>");
            target.OutputText.Should().Contain("<span style=\"color:#00F;background:#000\">b&amp;<br/>");
        }
    }
}