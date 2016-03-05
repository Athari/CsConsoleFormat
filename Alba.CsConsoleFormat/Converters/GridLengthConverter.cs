using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="GridLength"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"Auto" - <c>GridLength.Auto</c></item>
    /// <item>"*" - <c>GridLength.Star(1)</c></item>
    /// <item>"2*" - <c>GridLength.Star(2)</c></item>
    /// <item>"3", 3 - <c>GridLength.Char(3)</c></item>
    /// </list> 
    /// </summary>
    public class GridLengthConverter : TypeConverter
    {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) =>
            IsTypeStringOrNumeric(sourceType) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(null);
            else if (value is string)
                return FromString((string)value);
            else if (IsTypeNumeric(value.GetType()))
                return GridLength.Char(ToInt(value));
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is GridLength) || destinationType != typeof(string))
                throw GetConvertToException(value, destinationType);
            return ToString((GridLength)value, culture);
        }

        internal static string ToString (GridLength length, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.InvariantCulture;
            if (length.IsAuto)
                return Auto;
            else if (length.IsAbsolute)
                return length.Value.ToString(culture);
            else if (length.Value == 1)
                return Asterisk;
            else
                return length.Value.ToString(culture) + Asterisk;
        }

        private static GridLength FromString (string str)
        {
            int value;
            GridUnitType unitType;
            if (str.ToUpperInvariant() == AUTO) {
                unitType = GridUnitType.Auto;
                value = 0;
            }
            else if (str.EndsWith(Asterisk, StringComparison.Ordinal)) {
                unitType = GridUnitType.Star;
                value = str.Length == 1 ? 1 : ParseInt(str.Remove(str.Length - 1));
            }
            else {
                unitType = GridUnitType.Char;
                value = ParseInt(str);
            }
            return new GridLength(value, unitType);
        }
    }
}