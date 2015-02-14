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

        public Rect (Vector position, Size size)
        {
            this = size.IsEmpty ? Empty : new Rect(position.X, position.Y, size.Width, size.Height);
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
            get { return X; }
        }

        public int Top
        {
            get { return Y; }
        }

        public int Right
        {
            get { return IsEmpty ? int.MinValue : X + Width; }
        }

        public int Bottom
        {
            get { return IsEmpty ? int.MinValue : Y + Height; }
        }

        public Size Size
        {
            get { return IsEmpty ? Size.Empty : new Size(Width, Height); }
            set
            {
                if (value.IsEmpty) {
                    this = Empty;
                }
                else {
                    if (IsEmpty)
                        throw new InvalidOperationException("Cannot modify empty rect.");
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }

        public bool Contains (Point point)
        {
            return !IsEmpty && X <= point.X && point.X < Right && Y <= point.Y && point.Y < Bottom;
        }

        public bool Contains (int x, int y)
        {
            return Contains(new Point(x, y));
        }

        public bool IntersectsHorizontalLine (int y)
        {
            return !IsEmpty && Y <= y && y < Bottom;
        }

        public bool IntersectsVerticalLine (int x)
        {
            return !IsEmpty && X <= x && x < Right;
        }

        public Rect Deflate (Thickness th, bool throwOnError = false)
        {
            return new Rect(Left + th.Left, Top + th.Top, Width - th.Left - th.Right, Height - th.Top - th.Bottom, throwOnError);
        }

        public Rect Offset (Vector offset)
        {
            return new Rect(X + offset.X, Y + offset.Y, Width, Height);
        }

        public bool Equals (Rect other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
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
            return IsEmpty ? "Empty" : "{0} {1} {2} {3}".FmtInv(X, Y, Width, Height);
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