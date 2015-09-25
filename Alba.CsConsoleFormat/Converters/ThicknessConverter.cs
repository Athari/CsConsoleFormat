using System;
using System.ComponentModel;
using System.Globalization;

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
                    return new Thickness(Convert.ToInt32(value, CultureInfo.InvariantCulture));
            }
            return base.ConvertFrom(context, culture, value);
        }

        private static Thickness FromString (string str)
        {
            string[] parts = str.Split(new[] { ' ', ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            switch (parts.Length) {
                case 1:
                    return new Thickness(GetWidth(parts[0]));
                case 2:
                    return new Thickness(GetWidth(parts[0]), GetWidth(parts[1]));
                case 4:
                    return new Thickness(GetWidth(parts[0]), GetWidth(parts[1]), GetWidth(parts[2]), GetWidth(parts[3]));
                default:
                    throw new FormatException($"Invalid Thickness format: '{str}'.");
            }
        }

        private static int GetWidth (string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}