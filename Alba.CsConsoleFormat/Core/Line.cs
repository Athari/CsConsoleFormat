using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using static System.FormattableString;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "XAML requires writable members.")]
    public struct Line : IEquatable<Line>
    {
        private int _width;
        private int _height;

        public int X { get; set; }
        public int Y { get; set; }

        private Line(int x, int y, int width, int height, bool throwOnError = true)
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

        [Pure]
        public static Line Horizontal(int x, int y, int width, bool throwOnError = true) =>
            new Line(x, y, width, 0, throwOnError);

        [Pure]
        public static Line Vertical(int x, int y, int height, bool throwOnError = true) =>
            new Line(x, y, 0, height, throwOnError);

        public bool IsHorizontal => Width != 0;
        public bool IsVertical => Height != 0;

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Width cannot be negative.", nameof(value));
                if (value > 0 && Height != 0)
                    throw new ArgumentException("Vertical line cannot have non-zero width.", nameof(value));
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
                if (value > 0 && Width != 0)
                    throw new ArgumentException("Horizontal line cannot have non-zero height.", nameof(value));
                _height = value;
            }
        }

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public bool Equals(Line other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        public override bool Equals(object obj) => obj is Line && Equals((Line)obj);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        public override string ToString() =>
            Invariant($"{X} {Y} {(IsHorizontal ? Width : Height)} {(IsHorizontal ? "Horizontal" : IsVertical ? "Vertical" : "Empty")}");

        public static bool operator ==(Line left, Line right) => left.Equals(right);
        public static bool operator !=(Line left, Line right) => !left.Equals(right);
    }
}