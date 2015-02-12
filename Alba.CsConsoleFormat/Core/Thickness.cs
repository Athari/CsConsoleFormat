using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(ThicknessConverter))]
    public struct Thickness : IEquatable<Thickness>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Thickness (int left, int top, int right, int bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness (int width) : this(width, width, width, width)
        {}

        public bool Equals (Thickness other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals (object obj)
        {
            return obj is Thickness && Equals((Thickness)obj);
        }

        public override int GetHashCode ()
        {
            unchecked {
                int hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;
                return hashCode;
            }
        }

        public override string ToString ()
        {
            return "{0} {1} {2} {3}".FmtInv(Left, Top, Right, Bottom);
        }

        public static bool operator == (Thickness left, Thickness right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Thickness left, Thickness right)
        {
            return !left.Equals(right);
        }
    }
}