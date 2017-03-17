using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts length (represented by nullable <see cref="int"/>) to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"2" - <c>2</c></item>
    /// <item>"auto" - <c>null</c></item>
    /// </list> 
    /// </summary>
    public sealed class LengthConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || IsTypeStringOrNumeric(sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case object number when number.IsTypeNumeric():
                    return ToInt(number);
                case string auto when auto.ToUpperInvariant() == AUTO:
                    return null;
                case string str:
                    return ToInt(str);
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)) {
                switch (value) {
                    case null:
                        return Auto;
                    case int length:
                        return length.ToString();
                }
            }
            throw GetConvertToException(value, destinationType);
        }
    }
}