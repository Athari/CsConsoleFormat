using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
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
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return FromString((string)value);
            return base.ConvertFrom(context, culture, value);
        }

        private static Rect FromString (string str)
        {
            string[] parts = SplitNumbers(str, 4);
            if (parts.Length != 4)
                throw new FormatException($"Invalid Rect format: '{str}'.");
            return new Rect(ParseInt(parts[0]), ParseInt(parts[1]), ParseInt(parts[2]), ParseInt(parts[3]));
        }
    }
}