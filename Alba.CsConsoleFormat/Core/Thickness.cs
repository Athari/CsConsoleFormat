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

        public int Width
        {
            get { return Left + Right; }
        }

        public int Height
        {
            get { return Top + Bottom; }
        }

        public Size CollapsedThickness
        {
            get { return new Size(Left + Right, Top + Bottom); }
        }

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
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() & Bottom.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1} {2} {3}".FmtInv(Left, Top, Right, Bottom);
        }

        public static Thickness Add (Thickness left, Thickness right)
        {
            return new Thickness(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);
        }

        public static Thickness Subtract (Thickness left, Thickness right)
        {
            return new Thickness(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);
        }

        public static bool operator == (Thickness left, Thickness right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Thickness left, Thickness right)
        {
            return !left.Equals(right);
        }

        public static Thickness operator + (Thickness left, Thickness right)
        {
            return Add(left, right);
        }

        public static Thickness operator - (Thickness left, Thickness right)
        {
            return Subtract(left, right);
        }
    }
}