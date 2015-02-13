using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(PointConverter))]
    public struct Point
    {
        private int _x;
        private int _y;

        public Point (int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public bool Equals (Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals (object obj)
        {
            return obj is Point && Equals((Point)obj);
        }

        public override int GetHashCode ()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1}".FmtInv(X, Y);
        }

        public static Point Add (Point left, Vector right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point Subtract (Point left, Vector right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static bool operator == (Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Point left, Point right)
        {
            return !left.Equals(right);
        }

        public static Point operator + (Point left, Vector right)
        {
            return Add(left, right);
        }

        public static Point operator - (Point left, Vector right)
        {
            return Subtract(left, right);
        }
    }
}