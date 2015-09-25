using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Thickness"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"1 2 3 4" - <c>new Thickness(1, 2, 3, 4)</c></item>
    /// <item>"1 2" - <c>new Thickness(1, 2)</c> (<c>new Thickness(1, 2, 1, 2)</c>)</item>
    /// <item>"1", 1 - <c>new Thickness(1)</c> (<c>new Thickness(1, 1, 1, 1)</c>)</item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class ThicknessConverter : TypeConverter
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
                return new Thickness(ToInt(value));
            return base.ConvertFrom(context, culture, value);
        }

        private static Thickness FromString (string str)
        {
            string[] parts = SplitNumbers(str, 4);
            switch (parts.Length) {
                case 1:
                    return new Thickness(ParseInt(parts[0]));
                case 2:
                    return new Thickness(ParseInt(parts[0]), ParseInt(parts[1]));
                case 4:
                    return new Thickness(ParseInt(parts[0]), ParseInt(parts[1]), ParseInt(parts[2]), ParseInt(parts[3]));
                default:
                    throw new FormatException($"Invalid Thickness format: '{str}'.");
            }
        }
    }
}