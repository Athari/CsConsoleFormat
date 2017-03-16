using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [TypeConverter(typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        private static readonly GridLengthConverter _Converter = new GridLengthConverter();

        public int Value { get; }
        public GridUnitType UnitType { get; }

        public GridLength(int value, GridUnitType unitType)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be negative.", nameof(value));
            Value = unitType != GridUnitType.Auto ? value : 0;
            UnitType = unitType;
        }

        public static GridLength Auto => new GridLength(0, GridUnitType.Auto);
        public static GridLength Char(int value) => new GridLength(value, GridUnitType.Char);
        public static GridLength Star(int value) => new GridLength(value, GridUnitType.Star);

        public bool IsAbsolute => UnitType == GridUnitType.Char;
        public bool IsAuto => UnitType == GridUnitType.Auto;
        public bool IsStar => UnitType == GridUnitType.Star;

        public bool Equals(GridLength other) => Value == other.Value && UnitType == other.UnitType;
        public override bool Equals(object obj) => obj is GridLength && Equals((GridLength)obj);
        public override int GetHashCode() => Value.GetHashCode() ^ UnitType.GetHashCode();

        public override string ToString() => _Converter.ConvertToString(this) ?? "";

        public static bool operator ==(GridLength left, GridLength right) => left.Equals(right);
        public static bool operator !=(GridLength left, GridLength right) => !left.Equals(right);
    }
}