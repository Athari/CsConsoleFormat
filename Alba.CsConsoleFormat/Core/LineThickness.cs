using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

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

        public Thickness CharThickness
        {
            get { return new Thickness(Left.ToCharWidth(), Top.ToCharWidth(), Right.ToCharWidth(), Bottom.ToCharWidth()); }
        }

        public Size CollapsedCharThickness
        {
            get { return CharThickness.CollapsedThickness; }
        }

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
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() & Bottom.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1} {2} {3}".FmtInv(Left, Top, Right, Bottom);
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