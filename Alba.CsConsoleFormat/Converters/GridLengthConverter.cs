using System;
using System.ComponentModel;
using System.Globalization;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    public class GridLengthConverter : TypeConverter
    {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode type = Type.GetTypeCode(sourceType);
            return type == TypeCode.String || TypeCode.Int16 <= type && type <= TypeCode.Decimal || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(null);
            else if (value is string)
                return FromString((string)value);
            else {
                TypeCode type = Type.GetTypeCode(value.GetType());
                if (TypeCode.Int16 <= type && type <= TypeCode.Decimal)
                    return GridLength.Char(Convert.ToInt32(value, CultureInfo.InvariantCulture));
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is GridLength))
                throw GetConvertToException(value, destinationType);
            return ToString((GridLength)value, culture);
        }

        internal static string ToString (GridLength length, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.InvariantCulture;
            if (length.IsAuto)
                return "Auto";
            else if (length.IsAbsolute)
                return length.Value.ToString(culture);
            else if (length.Value == 1)
                return "*";
            else
                return length.Value.ToString(culture) + "*";
        }

        private static GridLength FromString (string str)
        {
            int value;
            GridUnitType unitType;
            if (str.ToUpperInvariant() == "AUTO") {
                unitType = GridUnitType.Auto;
                value = 0;
            }
            else if (str.EndsWith("*", StringComparison.Ordinal)) {
                unitType = GridUnitType.Star;
                value = str.Length == 1 ? 1 : GetValue(str.Remove(str.Length - 1));
            }
            else {
                unitType = GridUnitType.Char;
                value = GetValue(str);
            }
            return new GridLength(value, unitType);
        }

        private static int GetValue (string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}