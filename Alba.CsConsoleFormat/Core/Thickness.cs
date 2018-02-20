using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using static System.FormattableString;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(ThicknessConverter))]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "XAML requires writable members.")]
    public struct Thickness : IEquatable<Thickness>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(int vertical, int horizontal) : this(vertical, horizontal, vertical, horizontal)
        { }

        public Thickness(int width) : this(width, width, width, width)
        { }

        public bool IsEmpty => Left == 0 && Top == 0 && Right == 0 && Bottom == 0;

        public int Width => Left + Right;
        public int Height => Top + Bottom;

        public Size CollapsedThickness => new Size(Left + Right, Top + Bottom);

        public bool Equals(Thickness other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        public override bool Equals(object obj) => obj is Thickness && Equals((Thickness)obj);
        public override int GetHashCode() => Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() & Bottom.GetHashCode();

        public override string ToString() => Invariant($"{Left} {Top} {Right} {Bottom}");

        [Pure]
        public static Thickness Max(Thickness left, Thickness right) =>
            new Thickness(Math.Max(left.Left, right.Left), Math.Max(left.Top, right.Top), Math.Max(left.Right, right.Right), Math.Max(left.Bottom, right.Bottom));

        [Pure]
        public static Thickness Min(Thickness left, Thickness right) =>
            new Thickness(Math.Min(left.Left, right.Left), Math.Min(left.Top, right.Top), Math.Min(left.Right, right.Right), Math.Min(left.Bottom, right.Bottom));

        [Pure]
        public static Thickness Add(Thickness left, Thickness right) =>
            new Thickness(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);

        [Pure]
        public static Thickness Subtract(Thickness left, Thickness right) =>
            new Thickness(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);

        [Pure]
        public static Thickness Negate(Thickness left) =>
            new Thickness(-left.Left, -left.Top, -left.Right, -left.Bottom);

        public static bool operator ==(Thickness left, Thickness right) => left.Equals(right);
        public static bool operator !=(Thickness left, Thickness right) => !left.Equals(right);
        public static Thickness operator +(Thickness left, Thickness right) => Add(left, right);
        public static Thickness operator -(Thickness left, Thickness right) => Subtract(left, right);
        public static Thickness operator -(Thickness left) => Negate(left);
        public static implicit operator Thickness(int width) => new Thickness(width);
    }
}