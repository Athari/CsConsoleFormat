﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using static System.FormattableString;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(PointConverter))]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "XAML requires writable members.")]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Point && Equals((Point)obj);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() => Invariant($"{X} {Y}");

        [Pure]
        public static Point Add(Point left, Vector right) => new Point(left.X + right.X, left.Y + right.Y);

        [Pure]
        public static Point Subtract(Point left, Vector right) => new Point(left.X - right.X, left.Y - right.Y);

        public static bool operator ==(Point left, Point right) => left.Equals(right);
        public static bool operator !=(Point left, Point right) => !left.Equals(right);
        public static Point operator +(Point left, Vector right) => Add(left, right);
        public static Point operator -(Point left, Vector right) => Subtract(left, right);
    }
}