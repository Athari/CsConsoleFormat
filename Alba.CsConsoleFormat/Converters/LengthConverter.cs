using System;
using System.ComponentModel;
using System.Globalization;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts length (represented by nullable <see cref="int"/>) to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"2" - <c>2</c></item>
    /// <item>"auto" - <c>null</c></item>
    /// </list> 
    /// </summary>
    public class LengthConverter : TypeConverter
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
                return Convert.ToInt32(value, culture);
            str = str.ToUpperInvariant();
            return str == "AUTO" ? (object)null : Convert.ToInt32(str, culture);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value != null ? Convert.ToString(value, culture) : "Auto";
            else
                throw GetConvertToException(value, destinationType);
        }
    }
}