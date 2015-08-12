using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [TypeConverter (typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        private readonly int _value;
        private readonly GridUnitType _unitType;

        public GridLength (int value, GridUnitType unitType)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be negative.", "value");
            _value = unitType != GridUnitType.Auto ? value : 0;
            _unitType = unitType;
        }

        public static GridLength Char (int value)
        {
            return new GridLength(value, GridUnitType.Char);
        }

        public static GridLength Star (int value)
        {
            return new GridLength(value, GridUnitType.Star);
        }

        public static GridLength Auto
        {
            get { return new GridLength(0, GridUnitType.Auto); }
        }

        public bool IsAbsolute
        {
            get { return _unitType == GridUnitType.Char; }
        }

        public bool IsAuto
        {
            get { return _unitType == GridUnitType.Auto; }
        }

        public bool IsStar
        {
            get { return _unitType == GridUnitType.Star; }
        }

        public int Value
        {
            get { return _value; }
        }

        public GridUnitType UnitType
        {
            get { return (_unitType); }
        }

        public bool Equals (GridLength other)
        {
            return _value == other._value && _unitType == other._unitType;
        }

        public override bool Equals (object obj)
        {
            return obj is GridLength && Equals((GridLength)obj);
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode() ^ _unitType.GetHashCode();
        }

        public override string ToString ()
        {
            return GridLengthConverter.ToString(this);
        }
    }
}