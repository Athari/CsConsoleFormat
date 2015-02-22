using System;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    public struct Line : IEquatable<Line>
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        private Line (int x, int y, int width, int height, bool throwOnError = true)
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

        public static Line Horizontal (int x, int y, int width, bool throwOnError = true)
        {
            return new Line(x, y, width, 0, throwOnError);
        }

        public static Line Vertical (int x, int y, int height, bool throwOnError = true)
        {
            return new Line(x, y, 0, height, throwOnError);
        }

        public bool IsHorizontal
        {
            get { return Width != 0; }
        }

        public bool IsVertical
        {
            get { return Height != 0; }
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
                if (value > 0 && Height != 0)
                    throw new ArgumentException("Vertical line cannot have non-zero width.", "value");
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
                if (value > 0 && Width != 0)
                    throw new ArgumentException("Horizontal line cannot have non-zero height.", "value");
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

        public bool Equals (Line other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals (object obj)
        {
            return obj is Line && Equals((Line)obj);
        }

        public override int GetHashCode ()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString ()
        {
            return "{0} {1} {2} {3}".FmtInv(X, Y, IsHorizontal ? Width : Height, IsHorizontal ? "Horizontal" : "Vertical");
        }

        public static bool operator == (Line left, Line right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Line left, Line right)
        {
            return !left.Equals(right);
        }
    }
}