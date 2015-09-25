using System;
using System.ComponentModel;
using System.Globalization;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="ConsoleColor"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"Black", 0 - <c>ConsoleColor.Black</c></item>
    /// <item>"inherit" - <c>null</c></item>
    /// </list> 
    /// </summary>
    public class ConsoleColorConverter : TypeConverter
    {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode type = Type.GetTypeCode(sourceType);
            return type == TypeCode.String || TypeCode.Int16 <= type && type <= TypeCode.Decimal;
        }

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(null);
            var str = value as string;
            if (str == null)
                return (ConsoleColor)Convert.ToInt32(value, culture);
            str = str.ToUpperInvariant();
            return str == "INHERIT" ? (ConsoleColor?)null : (ConsoleColor)Enum.Parse(typeof(ConsoleColor), str, true);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value != null ? Convert.ToString(value, culture) : "Inherit";
            else
                throw GetConvertToException(value, destinationType);
        }
    }
}