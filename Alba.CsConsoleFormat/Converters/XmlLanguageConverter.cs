using System;
using System.ComponentModel;
using System.Globalization;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    /// <summary>Converts string/XmlLanguage.</summary>
    public class XmlLanguageConverter : TypeConverter
    {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;
            var str = value as string;
            if (str == null)
                throw GetConvertFromException(value);
            return new XmlLanguage(str);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;
            var lng = value as XmlLanguage;
            if (lng == null)
                throw GetConvertToException(value, destinationType);
            return lng.Name;
        }
    }
}