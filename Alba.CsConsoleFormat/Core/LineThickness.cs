using System;
using System.ComponentModel;
using static System.FormattableString;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(LineThicknessConverter))]
    public struct LineThickness : IEquatable<LineThickness>
    {
        public LineWidth Left { get; set; }
        public LineWidth Top { get; set; }
        public LineWidth Right { get; set; }
        public LineWidth Bottom { get; set; }

        public LineThickness (LineWidth left, LineWidth top, LineWidth right, LineWidth bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public LineThickness (LineWidth vertical, LineWidth horizontal) : this(vertical, horizontal, vertical, horizontal)
        {}

        public LineThickness (LineWidth width) : this(width, width, width, width)
        {}

        public static LineThickness None => new LineThickness(LineWidth.None);
        public static LineThickness Single => new LineThickness(LineWidth.Single);
        public static LineThickness Wide => new LineThickness(LineWidth.Wide);

        public Thickness CharThickness => new Thickness(Left.ToCharWidth(), Top.ToCharWidth(), Right.ToCharWidth(), Bottom.ToCharWidth());
        public Size CollapsedCharThickness => CharThickness.CollapsedThickness;

        public bool Equals (LineThickness other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        public override bool Equals (object obj) => obj is LineThickness && Equals((LineThickness)obj);
        public override int GetHashCode () => Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() & Bottom.GetHashCode();

        public override string ToString () => Invariant($"{Left} {Top} {Right} {Bottom}");

        public static bool operator == (LineThickness left, LineThickness right) => left.Equals(right);
        public static bool operator != (LineThickness left, LineThickness right) => !left.Equals(right);
    }
}