using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Size"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2" - <c>new Size(1, 2)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class SizeConverter : TypeConverter
    {
        private static readonly Lazy<ConstructorInfo> SizeConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Size).GetConstructor(new[] { typeof(int), typeof(int), typeof(bool) }));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            base.CanConvertTo(context, destinationType) || destinationType == typeof(InstanceDescriptor);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case string str:
                    return FromString(str);
                default:
                    return base.ConvertFrom(context, culture, value);
            }

            Size FromString(string str)
            {
                string[] parts = SplitNumbers(str, 2);
                if (parts.Length != 2)
                    throw new FormatException($"Invalid Size format: '{0}'.");
                return new Size(ParseInt(parts[0]), ParseInt(parts[1]));
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is Size size))
                throw GetConvertToException(value, destinationType);

            if (destinationType == typeof(string))
                return size.ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(SizeConstructor.Value, new object[] { size.Width, size.Height, false }, true);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}