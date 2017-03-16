using System;
using System.ComponentModel;
using static System.FormattableString;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(SizeConverter))]
    public struct Size : IEquatable<Size>
    {
        public const int Infinity = int.MaxValue;

        private int _width;
        private int _height;

        public Size(int width, int height, bool throwOnError = true)
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
            _width = width;
            _height = height;
        }

        public static Size Empty => new Size(0, 0);

        public bool IsEmpty => Width == 0 || Height == 0;
        public bool IsInfinite => IsHeightInfinite || IsWidthInfinite;
        public bool IsHeightInfinite => Height == Infinity;
        public bool IsWidthInfinite => Width == Infinity;

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

        public bool Equals(Size other) => _width == other._width && _height == other._height;
        public override bool Equals(object obj) => obj is Size && Equals((Size)obj);
        public override int GetHashCode() => Width.GetHashCode() ^ Height.GetHashCode();

        public override string ToString() => Invariant($"{_width} {_height}");

        public static Size Add(Size left, Size right) =>
            new Size(left.Width + right.Width, left.Height + right.Height);

        public static Size Add(Size left, Thickness right, bool throwOnError = false) =>
            new Size(left.Width + right.Width, left.Height + right.Height, throwOnError);

        public static Size Subtract(Size left, Size right, bool throwOnError = false) =>
            new Size(left.Width - right.Width, left.Height - right.Height, throwOnError);

        public static Size Subtract(Size left, Thickness right, bool throwOnError = false) =>
            new Size(left.Width - right.Width, left.Height - right.Height, throwOnError);

        public static Size Max(Size size1, Size size2) =>
            new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));

        public static Size Min(Size size1, Size size2) =>
            new Size(Math.Min(size1.Width, size2.Width), Math.Min(size1.Height, size2.Height));

        public static Size MinMax(Size size, Size min, Size max) =>
            new Size(MinMax(size.Width, min.Width, max.Width), MinMax(size.Height, min.Height, max.Height));

        private static int MinMax(int value, int min, int max) =>
            Math.Max(Math.Min(value, max), min);

        public static bool operator ==(Size left, Size right) => left.Equals(right);
        public static bool operator !=(Size left, Size right) => !left.Equals(right);
        public static Size operator +(Size left, Size right) => Add(left, right);
        public static Size operator +(Size left, Thickness right) => Add(left, right);
        public static Size operator -(Size left, Size right) => Subtract(left, right);
        public static Size operator -(Size left, Thickness right) => Subtract(left, right);
    }
}