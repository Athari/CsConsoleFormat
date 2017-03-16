using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Rect"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2 3 4" - <c>new Rect(1, 2, 3, 4)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class RectConverter : TypeConverter
    {
        private static readonly Lazy<ConstructorInfo> RectConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Rect).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }));

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

            Rect FromString(string str)
            {
                string[] parts = SplitNumbers(str, 4);
                if (parts.Length != 4)
                    throw new FormatException($"Invalid Rect format: '{str}'.");
                return new Rect(ParseInt(parts[0]), ParseInt(parts[1]), ParseInt(parts[2]), ParseInt(parts[3]));
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is Rect rect))
                throw GetConvertToException(value, destinationType);

            if (destinationType == typeof(string))
                return rect.ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(RectConstructor.Value, new object[] {
                    rect.X, rect.Y, rect.Width, rect.Height, false
                }, true);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}