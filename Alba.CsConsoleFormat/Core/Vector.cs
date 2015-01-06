using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(VectorConverter))]
    public struct Vector
    {
        private int _x;
        private int _y;

        public Vector (int x, int y)
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

        public bool Equals (Vector other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals (object obj)
        {
            return obj is Vector && Equals((Vector)obj);
        }

        public override int GetHashCode ()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1}".FmtInv(_x, _y);
        }

        public static bool operator == (Vector left, Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Vector left, Vector right)
        {
            return !left.Equals(right);
        }
    }
}