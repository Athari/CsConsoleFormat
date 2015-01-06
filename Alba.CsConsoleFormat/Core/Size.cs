using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(SizeConverter))]
    public struct Size : IEquatable<Size>
    {
        private static readonly Size _Empty = new Size { _width = Int32.MinValue, _height = Int32.MinValue };

        private int _width;
        private int _height;

        public Size (int width, int height)
        {
            if (width < 0)
                throw new ArgumentException("Width cannot be negative.", "width");
            if (height < 0)
                throw new ArgumentException("Height cannot be negative.", "height");
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

        public static bool operator == (Size left, Size right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Size left, Size right)
        {
            return !left.Equals(right);
        }
    }
}