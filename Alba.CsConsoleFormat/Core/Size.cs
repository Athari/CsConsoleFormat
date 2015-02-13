using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(SizeConverter))]
    public struct Size : IEquatable<Size>
    {
        private static readonly Size _Empty = new Size { _width = int.MinValue, _height = int.MinValue };

        private int _width;
        private int _height;

        public Size (int width, int height, bool throwOnError = true)
        {
            if (width < 0) {
                if (throwOnError)
                    throw new ArgumentException("Width cannot be negative.", "width");
                else
                    width = 0;
            }
            if (height < 0) {
                if (throwOnError)
                    throw new ArgumentException("Height cannot be negative.", "height");
                else
                    height = 0;
            }
            _width = width;
            _height = height;
        }

        public static Size Empty
        {
            get { return _Empty; }
        }

        public bool IsEmpty
        {
            get { return Width < 0; }
        }

        public bool IsInfinite
        {
            get { return Width == int.MaxValue || Height == int.MaxValue; }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty size.");
                if (value < 0)
                    throw new ArgumentException("Width cannot be negative.", "value");
                _width = value;
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty size.");
                if (value < 0)
                    throw new ArgumentException("Height cannot be negative.", "value");
                _height = value;
            }
        }

        public bool Equals (Size other)
        {
            return _width == other._width && _height == other._height;
        }

        public override bool Equals (object obj)
        {
            return obj is Size && Equals((Size)obj);
        }

        public override int GetHashCode ()
        {
            return IsEmpty ? 0 : Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString ()
        {
            return IsEmpty ? "Empty" : "{0} {1}".FmtInv(_width, _height);
        }

        public static Size Add (Size left, Size right)
        {
            return new Size(left.Width + right.Width, left.Height + right.Height);
        }

        public static Size Add (Size left, Thickness right, bool throwOnError = false)
        {
            return new Size(left.Width + right.Width, left.Height + right.Height, throwOnError);
        }

        public static Size Subtract (Size left, Size right, bool throwOnError = false)
        {
            return new Size(left.Width - right.Width, left.Height - right.Height, throwOnError);
        }

        public static Size Max (Size size1, Size size2)
        {
            return new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
        }

        public static Size Min (Size size1, Size size2)
        {
            return new Size(Math.Min(size1.Width, size2.Width), Math.Min(size1.Height, size2.Height));
        }

        public static Size MinMax (Size size, Size min, Size max)
        {
            return new Size(MinMax(size.Width, min.Width, max.Width), MinMax(size.Height, min.Height, max.Height));
        }

        private static int MinMax (int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        public static bool operator == (Size left, Size right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Size left, Size right)
        {
            return !left.Equals(right);
        }

        public static Size operator + (Size left, Size right)
        {
            return Add(left, right);
        }

        public static Size operator + (Size left, Thickness right)
        {
            return Add(left, right);
        }

        public static Size operator - (Size left, Size right)
        {
            return Subtract(left, right);
        }
    }
}