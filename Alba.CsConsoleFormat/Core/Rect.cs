using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(RectConverter))]
    public struct Rect : IEquatable<Rect>
    {
        private static readonly Rect _Empty = new Rect { _x = int.MaxValue, _y = int.MaxValue, _width = int.MinValue, _height = int.MinValue };

        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public Rect (int x, int y, int width, int height, bool throwOnError = true)
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
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public Rect (Size size)
        {
            if (size.IsEmpty) {
                this = Empty;
            }
            else {
                _x = _y = 0;
                _width = size.Width;
                _height = size.Height;
            }
        }

        public static Rect Empty
        {
            get { return _Empty; }
        }

        public bool IsEmpty
        {
            get { return Width < 0; }
        }

        public int X
        {
            get { return _x; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty rect.");
                _x = value;
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty rect.");
                _y = value;
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty rect.");
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
                    throw new InvalidOperationException("Cannot modify empty rect.");
                if (value < 0)
                    throw new ArgumentException("Height cannot be negative.", "value");
                _height = value;
            }
        }

        public int Left
        {
            get { return _x; }
        }

        public int Top
        {
            get { return _y; }
        }

        public int Right
        {
            get { return IsEmpty ? int.MinValue : _x + _width; }
        }

        public int Bottom
        {
            get { return IsEmpty ? int.MinValue : _y + _height; }
        }

        public Size Size
        {
            get { return IsEmpty ? Size.Empty : new Size(_width, _height); }
            set
            {
                if (value.IsEmpty) {
                    this = _Empty;
                }
                else {
                    if (IsEmpty)
                        throw new InvalidOperationException("Cannot modify empty rect.");
                    _width = value.Width;
                    _height = value.Height;
                }
            }
        }

        public Rect Deflate (Thickness th, bool throwOnError = false)
        {
            return new Rect(Left + th.Left, Top + th.Top, Width - th.Left - th.Right, Height - th.Top - th.Bottom, throwOnError);
        }

        public bool Equals (Rect other)
        {
            return _x == other._x && _y == other._y && _width == other._width && _height == other._height;
        }

        public override bool Equals (object obj)
        {
            return obj is Rect && Equals((Rect)obj);
        }

        public override int GetHashCode ()
        {
            return IsEmpty ? 0 : X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString ()
        {
            return IsEmpty ? "Empty" : "{0} {1} {2} {3}".FmtInv(_x, _y, _width, _height);
        }

        public static bool operator == (Rect left, Rect right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Rect left, Rect right)
        {
            return !left.Equals(right);
        }
    }
}