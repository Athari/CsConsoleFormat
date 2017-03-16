using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="XmlLanguage"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"en-us" - <c>new XmlLanguage("en-us")</c></item>
    /// </list> 
    /// </summary>
    public class XmlLanguageConverter : TypeConverter
    {
        private static readonly Lazy<ConstructorInfo> XmlLanguageConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(XmlLanguage).GetConstructor(new[] { typeof(string) }));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            base.CanConvertTo(context, destinationType) || destinationType == typeof(InstanceDescriptor);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case string str:
                    return new XmlLanguage(str);
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is XmlLanguage lng))
                throw GetConvertToException(value, destinationType);

            if (destinationType == typeof(string))
                return lng.ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(XmlLanguageConstructor.Value, new object[] { lng.Name }, true);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}