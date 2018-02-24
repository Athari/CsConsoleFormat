using System;
using Alba.CsConsoleFormat.Testing.Xunit;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class LineTests
    {
        [Fact]
        public void CreateProperties()
        {
            var line = new Line { X = 1, Y = 2, Width = 10 };
            line.X.Should().Be(1);
            line.Y.Should().Be(2);
        }

        [Fact]
        public void CreateNegativeSize()
        {
            const string width = nameof(width);
            const string height = nameof(height);

            new Action(() => _ = Line.Horizontal(1, 2, -10)).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(width);
            new Action(() => _ = Line.Vertical(-1, -2, -10)).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(height);
        }

        [Fact]
        public void CreateNegativeSizeNoThrow()
        {
            Line.Horizontal(1, 2, -10, false).Width.Should().Be(0);
            Line.Vertical(-1, -2, -10, false).Height.Should().Be(0);
        }

        [Fact]
        public void ChangeNegativeSize()
        {
            const string value = nameof(value);

            Line horizontal = Line.Horizontal(1, 2, 10);
            Line vertical = Line.Vertical(-1, -2, 10);

            new Action(() => horizontal.Width = -10).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(value);
            new Action(() => vertical.Height = -10).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(value);
        }

        [Fact]
        public void ChangeDirection()
        {
            const string value = nameof(value);

            Line horizontal = Line.Horizontal(1, 2, 10);
            Line vertical = Line.Vertical(-1, -2, 10);

            new Action(() => horizontal.Height = 10).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(value);
            new Action(() => vertical.Width = 10).Should().Throw<ArgumentException>().Which.ParamName.Should().Be(value);
        }

        [Fact]
        public void EmptyLine()
        {
            var line = new Line();

            line.X.Should().Be(0);
            line.Y.Should().Be(0);
            line.Width.Should().Be(0);
            line.Height.Should().Be(0);
            line.Left.Should().Be(0);
            line.Top.Should().Be(0);
            line.Right.Should().Be(0);
            line.Bottom.Should().Be(0);
            line.IsHorizontal.Should().BeFalse();
            line.IsVertical.Should().BeFalse();
            line.Position.Should().Be(new Point(0, 0));
        }

        [Fact]
        public void HorizontalLine()
        {
            var line = Line.Horizontal(1, 2, 10);

            line.X.Should().Be(1);
            line.Y.Should().Be(2);
            line.Width.Should().Be(10);
            line.Height.Should().Be(0);
            line.Left.Should().Be(1);
            line.Top.Should().Be(2);
            line.Right.Should().Be(11);
            line.Bottom.Should().Be(2);
            line.IsHorizontal.Should().BeTrue();
            line.IsVertical.Should().BeFalse();
            line.Position.Should().Be(new Point(1, 2));
        }

        [Fact]
        public void VerticalLine()
        {
            var line = Line.Vertical(-1, -2, 10);

            line.X.Should().Be(-1);
            line.Y.Should().Be(-2);
            line.Width.Should().Be(0);
            line.Height.Should().Be(10);
            line.Left.Should().Be(-1);
            line.Top.Should().Be(-2);
            line.Right.Should().Be(-1);
            line.Bottom.Should().Be(8);
            line.IsHorizontal.Should().BeFalse();
            line.IsVertical.Should().BeTrue();
            line.Position.Should().Be(new Point(-1, -2));
        }

        [Fact]
        public void ChangePosition()
        {
            var line = Line.Horizontal(1, 2, 10);

            line.Position = new Point(3, 4);

            line.X.Should().Be(3);
            line.Y.Should().Be(4);
            line.Position.Should().Be(new Point(3, 4));
        }

        [Fact]
        public void ChangeSize()
        {
            var line = Line.Vertical(-1, -2, 10);

            line.Width = 0;
            line.Height = 11;

            line.Height.Should().Be(11);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(1, 2, 10, 1, 2, 10)]
        public void EqualsHorizontalIsTrue(int p1x, int p1y, int w1, int p2x, int p2y, int w2)
        {
            var line1 = Line.Horizontal(p1x, p1y, w1);
            var line2 = Line.Horizontal(p2x, p2y, w2);

            (Equals(line1, line2)).Should().BeTrue();
            (line1 == line2).Should().BeTrue();
            (line1 != line2).Should().BeFalse();
            (line1.GetHashCode() == line2.GetHashCode()).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(1, 2, 10, 1, 2, 10)]
        public void EqualsVerticalIsTrue(int p1x, int p1y, int h1, int p2x, int p2y, int h2)
        {
            var line1 = Line.Vertical(p1x, p1y, h1);
            var line2 = Line.Vertical(p2x, p2y, h2);

            (Equals(line1, line2)).Should().BeTrue();
            (line1 == line2).Should().BeTrue();
            (line1 != line2).Should().BeFalse();
            (line1.GetHashCode() == line2.GetHashCode()).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 0, 1, 1, 1)]
        [InlineData(1, 2, 10, 1, 6, 10)]
        [InlineData(1, 2, 10, 6, 2, 10)]
        [InlineData(1, 2, 10, 1, 2, 16)]
        public void EqualsHorizontalIsFalse(int p1x, int p1y, int w1, int p2x, int p2y, int w2)
        {
            var line1 = Line.Horizontal(p1x, p1y, w1);
            var line2 = Line.Horizontal(p2x, p2y, w2);

            (Equals(line1, line2)).Should().BeFalse();
            (line1 == line2).Should().BeFalse();
            (line1 != line2).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 0, 1, 1, 1)]
        [InlineData(1, 2, 10, 1, 6, 10)]
        [InlineData(1, 2, 10, 6, 2, 10)]
        [InlineData(1, 2, 10, 1, 2, 16)]
        public void EqualsVerticalIsFalse(int p1x, int p1y, int h1, int p2x, int p2y, int h2)
        {
            var line1 = Line.Vertical(p1x, p1y, h1);
            var line2 = Line.Vertical(p2x, p2y, h2);

            (Equals(line1, line2)).Should().BeFalse();
            (line1 == line2).Should().BeFalse();
            (line1 != line2).Should().BeTrue();
        }

        [Theory, UseCrazyCulture]
        [InlineData(0, 0, 0, "0 0 0 Empty")]
        [InlineData(1, 2, 10, "1 2 10 Horizontal")]
        [InlineData(-1, -2, 10, "-1 -2 10 Horizontal")]
        public void ToStringHorizontal(int px, int py, int w, string str)
        {
            var line = Line.Horizontal(px, py, w);

            line.ToString().Should().Be(str);
        }

        [Theory, UseCrazyCulture]
        [InlineData(0, 0, 0, "0 0 0 Empty")]
        [InlineData(1, 2, 10, "1 2 10 Vertical")]
        [InlineData(-1, -2, 10, "-1 -2 10 Vertical")]
        public void ToStringVertical(int px, int py, int h, string str)
        {
            var line = Line.Vertical(px, py, h);

            line.ToString().Should().Be(str);
        }
    }
}