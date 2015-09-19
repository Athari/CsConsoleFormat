using System.ComponentModel;
using static System.FormattableString;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(VectorConverter))]
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector (int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals (Vector other) => X == other.X && Y == other.Y;
        public override bool Equals (object obj) => obj is Vector && Equals((Vector)obj);
        public override int GetHashCode () => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString () => Invariant($"{X} {Y}");

        public static Vector Add (Vector left, Vector right) => new Vector(left.X + right.X, left.Y + right.Y);
        public static Vector Subtract (Vector left, Vector right) => new Vector(left.X - right.X, left.Y - right.Y);
        public static Vector Negate (Vector self) => new Vector(-self.X, -self.Y);

        public static bool operator == (Vector left, Vector right) => left.Equals(right);
        public static bool operator != (Vector left, Vector right) => !left.Equals(right);
        public static Vector operator + (Vector left, Vector right) => Add(left, right);
        public static Vector operator - (Vector left, Vector right) => Subtract(left, right);
        public static Vector operator - (Vector self) => Negate(self);
    }
}