using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        private static readonly GridLengthConverter _Converter = new GridLengthConverter();

        public int Value { get; }
        public GridUnit Unit { get; }

        public GridLength(int value, GridUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be negative.", nameof(value));
            Value = unit != GridUnit.Auto ? value : 0;
            Unit = unit;
        }

        public static GridLength Auto => new GridLength(0, GridUnit.Auto);

        [Pure]
        public static GridLength Char(int value) => new GridLength(value, GridUnit.Char);

        [Pure]
        public static GridLength Star(int value) => new GridLength(value, GridUnit.Star);

        public bool IsAbsolute => Unit == GridUnit.Char;
        public bool IsAuto => Unit == GridUnit.Auto;
        public bool IsStar => Unit == GridUnit.Star;

        public bool Equals(GridLength other) => Value == other.Value && Unit == other.Unit;
        public override bool Equals(object obj) => obj is GridLength && Equals((GridLength)obj);
        public override int GetHashCode() => Value.GetHashCode() ^ Unit.GetHashCode();

        public override string ToString() => _Converter.ConvertToString(this) ?? "";

        public static bool operator ==(GridLength left, GridLength right) => left.Equals(right);
        public static bool operator !=(GridLength left, GridLength right) => !left.Equals(right);
    }
}