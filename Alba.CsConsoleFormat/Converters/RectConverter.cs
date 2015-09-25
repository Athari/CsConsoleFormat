using System;
using System.ComponentModel;
using System.Globalization;

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
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode type = Type.GetTypeCode(sourceType);
            return type == TypeCode.String || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return FromString((string)value);
            return base.ConvertFrom(context, culture, value);
        }

        private static Rect FromString (string str)
        {
            string[] parts = str.Split(new[] { ' ', ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
                throw new FormatException($"Invalid Rect format: '{str}'.");
            return new Rect(GetValue(parts[0]), GetValue(parts[1]), GetValue(parts[2]), GetValue(parts[3]));
        }

        private static int GetValue (string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}