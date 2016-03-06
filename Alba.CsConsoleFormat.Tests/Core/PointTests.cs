using Alba.CsConsoleFormat.Testing.Xunit;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class PointTests
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(2, 3, 5, 6, 7, 9)]
        [InlineData(2, 3, -5, -6, -3, -3)]
        public void Add_Vector(int px, int py, int vx, int vy, int rx, int ry)
        {
            var p = new Point(px, py);
            var v = new Vector(vx, vy);

            (p + v).Should().Be(new Point(rx, ry));
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(2, 3, 5, 6, -3, -3)]
        [InlineData(2, 3, -5, -6, 7, 9)]
        public void Subtract_Vector(int px, int py, int vx, int vy, int rx, int ry)
        {
            var p = new Point(px, py);
            var v = new Vector(vx, vy);

            (p - v).Should().Be(new Point(rx, ry));
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 2, 1, 2)]
        public void Equals_IsTrue(int p1x, int p1y, int p2x, int p2y)
        {
            var p1 = new Point(p1x, p1y);
            var p2 = new Point(p2x, p2y);

            (p1 == p2).Should().BeTrue();
            (p1 != p2).Should().BeFalse();
            (p1.GetHashCode() == p2.GetHashCode()).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 1, 1)]
        [InlineData(1, 2, 2, 1)]
        public void Equals_IsFalse(int p1x, int p1y, int p2x, int p2y)
        {
            var p1 = new Point(p1x, p1y);
            var p2 = new Point(p2x, p2y);

            (p1 == p2).Should().BeFalse();
            (p1 != p2).Should().BeTrue();
        }

        [Theory, UseCrazyCulture]
        [InlineData(0, 0, "0 0")]
        [InlineData(101, 2001, "101 2001")]
        [InlineData(-101, -2001, "-101 -2001")]
        public void ToString(int px, int py, string str)
        {
            var p = new Point(px, py);

            p.ToString().Should().Be(str);
        }
    }
}