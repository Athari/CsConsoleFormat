using System;
using System.ComponentModel;
using Alba.CsConsoleFormat.Framework.Text;

// TODO Make sure separate Rect.Empty values is actually needed, also check other core values
namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(RectConverter))]
    public struct Rect : IEquatable<Rect>
    {
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
            : this(position.X, position.Y, size.Width, size.Height)
        {}

        public Rect (Size size)
        {
            _x = _y = 0;
            _width = size.Width;
            _height = size.Height;
        }

        public static Rect FromBounds (int left, int top, int right, int bottom, bool throwOnError = true)
        {
            return new Rect(left, top, right - left, bottom - top, throwOnError);
        }

        public static Rect Empty
        {
            get { return new Rect(0, 0, 0, 0); }
        }

        public bool IsEmpty
        {
            get { return Width == 0 || Height == 0; }
        }

        public bool IsInfinite
        {
            get { return Width == Size.Infinity || Height == Size.Infinity; }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Width
        {
            get { return _width; }
            set
            {
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
            get { return X + Width; }
        }

        public int Bottom
        {
            get { return Y + Height; }
        }

        public Point Position
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public bool Contains (Point point)
        {
            return X <= point.X && point.X < Right && Y <= point.Y && point.Y < Bottom;
        }

        public bool Contains (int x, int y)
        {
            return Contains(new Point(x, y));
        }

        public bool IntersectsHorizontalLine (int y)
        {
            return Y <= y && y < Bottom;
        }

        public bool IntersectsVerticalLine (int x)
        {
            return X <= x && x < Right;
        }

        public bool IntersectsWith (Rect rect)
        {
            return Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;
        }

        public Rect Deflate (Thickness th, bool throwOnError = false)
        {
            return new Rect(Left + th.Left, Top + th.Top, Width - th.Left - th.Right, Height - th.Top - th.Bottom, throwOnError);
        }

        public Rect Deflate (Size size, bool throwOnError = false)
        {
            return new Rect(Left + size.Width, Top + size.Height, Width - size.Width * 2, Height - size.Height * 2, throwOnError);
        }

        public Rect Inflate (Thickness th, bool throwOnError = false)
        {
            return new Rect(Left - th.Left, Top - th.Top, Width + th.Left + th.Right, Height + th.Top + th.Bottom, throwOnError);
        }

        public Rect Inflate (Size size, bool throwOnError = false)
        {
            return new Rect(Left - size.Width, Top - size.Height, Width + size.Width * 2, Height + size.Height * 2, throwOnError);
        }

        public Rect Intersect (Rect rect)
        {
            return FromBounds(Math.Max(Left, rect.Left), Math.Max(Top, rect.Top), Math.Min(Right, rect.Right), Math.Min(Bottom, rect.Bottom), false);
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1} {2} {3}".FmtInv(X, Y, Width, Height);
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