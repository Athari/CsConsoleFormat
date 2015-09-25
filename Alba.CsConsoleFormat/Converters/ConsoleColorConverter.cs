using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

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
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) => IsTypeStringOrNumeric(sourceType);

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(null);
            var str = value as string;
            if (str == null)
                return ToEnum<ConsoleColor>(value);
            str = str.ToUpperInvariant();
            return str == INHERIT ? (ConsoleColor?)null : ParseEnum<ConsoleColor>(str);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value != null ? Convert.ToString(value, culture) : Inherit;
            throw GetConvertToException(value, destinationType);
        }
    }
}