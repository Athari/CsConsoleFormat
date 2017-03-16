using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Vector"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2" - <c>new Vector(1, 2)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class VectorConverter : TypeConverter
    {
        private static readonly Lazy<ConstructorInfo> VectorConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Vector).GetConstructor(new[] { typeof(int), typeof(int) }));

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

            Vector FromString(string str)
            {
                string[] parts = SplitNumbers(str, 2);
                if (parts.Length != 2)
                    throw new FormatException($"Invalid Vector format: '{str}'.");
                return new Vector(ParseInt(parts[0]), ParseInt(parts[1]));
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is Vector vector))
                throw GetConvertToException(value, destinationType);

            if (destinationType == typeof(string))
                return vector.ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(VectorConstructor.Value, new object[] { vector.X, vector.Y }, true);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}