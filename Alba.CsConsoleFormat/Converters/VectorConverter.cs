using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
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
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return FromString((string)value);
            return base.ConvertFrom(context, culture, value);
        }

        private static Vector FromString (string str)
        {
            string[] parts = SplitNumbers(str, 2);
            if (parts.Length != 2)
                throw new FormatException($"Invalid Vector format: '{str}'.");
            return new Vector(ParseInt(parts[0]), ParseInt(parts[1]));
        }
    }
}