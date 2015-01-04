using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(LineThicknessConverter))]
    public struct LineThickness : IEquatable<LineThickness>
    {
        public LineWidth Left { get; set; }
        public LineWidth Top { get; set; }
        public LineWidth Right { get; set; }
        public LineWidth Bottom { get; set; }

        public LineThickness (LineWidth left, LineWidth top, LineWidth right, LineWidth bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public LineThickness (LineWidth width) : this(width, width, width, width)
        {}

        public bool Equals (LineThickness other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals (object obj)
        {
            return obj is LineThickness && Equals((LineThickness)obj);
        }

        public override int GetHashCode ()
        {
            unchecked {
                int hashCode = (int)Left;
                hashCode = (hashCode * 397) ^ (int)Top;
                hashCode = (hashCode * 397) ^ (int)Right;
                hashCode = (hashCode * 397) ^ (int)Bottom;
                return hashCode;
            }
        }

        public static bool operator == (LineThickness left, LineThickness right)
        {
            return left.Equals(right);
        }

        public static bool operator != (LineThickness left, LineThickness right)
        {
            return !left.Equals(right);
        }
    }
}