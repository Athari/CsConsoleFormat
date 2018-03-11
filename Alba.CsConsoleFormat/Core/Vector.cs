using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using static System.FormattableString;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(VectorConverter))]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "XAML requires writable members.")]
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector(Point point) : this(point.X, point.Y)
        { }

        public bool Equals(Vector other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Vector && Equals((Vector)obj);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() => Invariant($"{X} {Y}");

        [Pure]
        public static Vector Add(Vector left, Vector right) => new Vector(left.X + right.X, left.Y + right.Y);

        [Pure]
        public static Vector Subtract(Vector left, Vector right) => new Vector(left.X - right.X, left.Y - right.Y);

        [Pure]
        public static Vector Negate(Vector self) => new Vector(-self.X, -self.Y);

        public static bool operator ==(Vector left, Vector right) => left.Equals(right);
        public static bool operator !=(Vector left, Vector right) => !left.Equals(right);
        public static Vector operator +(Vector left, Vector right) => Add(left, right);
        public static Vector operator -(Vector left, Vector right) => Subtract(left, right);
        public static Vector operator -(Vector self) => Negate(self);
    }
}