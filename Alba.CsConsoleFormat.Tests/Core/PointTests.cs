using Alba.CsConsoleFormat.Testing.Xunit;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests.Core
{
    public class PointTests
    {
        [Theory]
        [InlineData (0, 0, 0, 0, 0, 0)]
        [InlineData (2, 3, 5, 6, 7, 9)]
        [InlineData (2, 3, -5, -6, -3, -3)]
        public void AddVector (int px, int py, int vx, int vy, int rx, int ry)
        {
            (new Point(px, py) + new Vector(vx, vy)).Should().Be(new Point(rx, ry));
        }

        [Theory]
        [InlineData (0, 0, 0, 0, 0, 0)]
        [InlineData (2, 3, 5, 6, -3, -3)]
        [InlineData (2, 3, -5, -6, 7, 9)]
        public void SubtractVector (int px, int py, int vx, int vy, int rx, int ry)
        {
            (new Point(px, py) - new Vector(vx, vy)).Should().Be(new Point(rx, ry));
        }

        [Theory]
        [InlineData (0, 0, 0, 0, true)]
        [InlineData (1, 2, 1, 2, true)]
        [InlineData (1, 2, 2, 1, false)]
        public void Equals (int p1x, int p1y, int p2x, int p2y, bool equals)
        {
            (new Point(p1x, p1y) == new Point(p2x, p2y)).Should().Be(equals);
        }

        [Theory]
        [InlineData (0, 0, 0, 0, false)]
        [InlineData (1, 2, 1, 2, false)]
        [InlineData (1, 2, 2, 1, true)]
        public void NotEquals (int p1x, int p1y, int p2x, int p2y, bool equals)
        {
            (new Point(p1x, p1y) != new Point(p2x, p2y)).Should().Be(equals);
        }

        [Theory, UseCrazyCulture]
        [InlineData (0, 0, "0 0")]
        [InlineData (101, 2001, "101 2001")]
        [InlineData (-101, -2001, "-101 -2001")]
        public void ToString (int px, int py, string str)
        {
            new Point(px, py).ToString().Should().Be(str);
        }
    }
}