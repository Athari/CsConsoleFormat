using System;
using System.ComponentModel;
using System.Globalization;
using Alba.CsConsoleFormat.Framework.Text;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts between a sequence of <see cref="string">Strings</see> and <see cref="LineThickness"/>:
    /// "1"/<c>{ 1, 1, 1, 1 }</c>, "1 2"/<c>{ 1, 2, 1, 2 }</c>, "1 2 3 4"/<c>{ 1, 2, 3, 4 }</c> (left, top, right, bottom).
    /// Separator can be " " or ",".
    /// </summary>
    public class LineThicknessConverter : TypeConverter
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
            if (value is string)
                return FromString((string)value);
            if (value is LineWidth)
                return new LineThickness((LineWidth)value);
            return new LineThickness((LineWidth)Convert.ToInt32(value, CultureInfo.InvariantCulture));
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var thickness = (LineThickness)value;
            if (destinationType == typeof(string))
                return ToString(thickness);
            throw GetConvertToException(value, destinationType);
        }

        private static LineThickness FromString (string str)
        {
            string[] strParts = str.Split(new[] { ' ', ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            switch (strParts.Length) {
                case 1:
                    return new LineThickness(GetWidth(strParts[0]));
                case 2:
                    return new LineThickness(GetWidth(strParts[0]), GetWidth(strParts[1]), GetWidth(strParts[0]), GetWidth(strParts[1]));
                case 4:
                    return new LineThickness(GetWidth(strParts[0]), GetWidth(strParts[1]), GetWidth(strParts[2]), GetWidth(strParts[3]));
                default:
                    throw new FormatException("Invalid LineThickness format: \"{0}\"".Fmt(str));
            }
        }

        private static string ToString (LineThickness thickness)
        {
            return "{0} {1} {2} {3}".FmtInv(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        private static LineWidth GetWidth (string str)
        {
            return (LineWidth)Enum.Parse(typeof(LineWidth), str, true);
        }
    }
}