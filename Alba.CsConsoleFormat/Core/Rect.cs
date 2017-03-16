using System;
using System.ComponentModel;
using static System.FormattableString;

// ReSharper disable NonReadonlyMemberInGetHashCode
// TODO Make sure separate Rect.Empty values is actually needed, also check other core values
namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(RectConverter))]
    public struct Rect : IEquatable<Rect>
    {
        private int _width;
        private int _height;

        public int X { get; set; }
        public int Y { get; set; }

        public Rect(int x, int y, int width, int height, bool throwOnError = true)
        {
            if (width < 0) {
                if (throwOnError)
                    throw new ArgumentException("Width cannot be negative.", nameof(width));
                else
                    width = 0;
            }
            if (height < 0) {
                if (throwOnError)
                    throw new ArgumentException("Height cannot be negative.", nameof(height));
                else
                    height = 0;
            }
            X = x;
            Y = y;
            _width = width;
            _height = height;
        }

        public Rect(Vector position, Size size) : this(position.X, position.Y, size.Width, size.Height)
        {}

        public Rect(Point position, Size size) : this(position.X, position.Y, size.Width, size.Height)
        {}

        public Rect(Size size)
        {
            X = Y = 0;
            _width = size.Width;
            _height = size.Height;
        }

        public static Rect FromBounds(int left, int top, int right, int bottom, bool throwOnError = true) =>
            new Rect(left, top, right - left, bottom - top, throwOnError);

        public static Rect Empty => new Rect(0, 0, 0, 0);

        public bool IsEmpty => Width == 0 || Height == 0;
        public bool IsInfinite => Width == Size.Infinity || Height == Size.Infinity;

        public int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Width cannot be negative.", nameof(value));
                _width = value;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Height cannot be negative.", nameof(value));
                _height = value;
            }
        }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Line LeftLine => Line.Vertical(X, Y, Height);
        public Line TopLine => Line.Horizontal(X, Y, Width);
        public Line RightLine => Line.Vertical(Right - 1, Y, Height);
        public Line BottomLine => Line.Horizontal(X, Bottom - 1, Width);

        public bool Contains(Point point) => X <= point.X && point.X < Right && Y <= point.Y && point.Y < Bottom;
        public bool Contains(int x, int y) => Contains(new Point(x, y));

        public bool IntersectsHorizontalLine(int y) => Y <= y && y < Bottom;
        public bool IntersectsVerticalLine(int x) => X <= x && x < Right;
        public bool IntersectsWith(Rect rect) => Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;

        public Rect Deflate(Thickness by, bool throwOnError = false) =>
            new Rect(Left + by.Left, Top + by.Top, Width - by.Left - by.Right, Height - by.Top - by.Bottom, throwOnError);

        public Rect Deflate(Size size, bool throwOnError = false) =>
            new Rect(Left + size.Width, Top + size.Height, Width - size.Width * 2, Height - size.Height * 2, throwOnError);

        public Rect Inflate(Thickness by, bool throwOnError = false) =>
            new Rect(Left - by.Left, Top - by.Top, Width + by.Left + by.Right, Height + by.Top + by.Bottom, throwOnError);

        public Rect Inflate(Size by, bool throwOnError = false) =>
            new Rect(Left - by.Width, Top - by.Height, Width + by.Width * 2, Height + by.Height * 2, throwOnError);

        public Rect Intersect(Rect rect) =>
            FromBounds(Math.Max(Left, rect.Left), Math.Max(Top, rect.Top), Math.Min(Right, rect.Right), Math.Min(Bottom, rect.Bottom), false);

        public Rect Offset(Vector by) =>
            new Rect(X + by.X, Y + by.Y, Width, Height);

        public bool Equals(Rect other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        public override bool Equals(object obj) => obj is Rect && Equals((Rect)obj);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        public override string ToString() => Invariant($"{X} {Y} {Width} {Height}");

        public static bool operator ==(Rect left, Rect right) => left.Equals(right);
        public static bool operator !=(Rect left, Rect right) => !left.Equals(right);
    }
}