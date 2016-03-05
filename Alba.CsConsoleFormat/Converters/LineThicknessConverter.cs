using System;
using System.ComponentModel;
using System.Globalization;
using static Alba.CsConsoleFormat.TypeConverterUtils;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="LineThickness"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"None Single Wide Single", "0 1 2 0" - <c>new LineThickness(None, Single, Wide, Single)</c></item>
    /// <item>"Single Wide", "1 2" - <c>new LineThickness(Single, Wide)</c> (<c>new LineThickness(Single, Wide, Single, Wide)</c>)</item>
    /// <item>"Wide", "2", 2 - <c>new LineThickness(Wide)</c> (<c>new LineThickness(Wide, Wide, Wide, Wide)</c>)</item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class LineThicknessConverter : TypeConverter
    {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) =>
            IsTypeStringOrNumeric(sourceType) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(null);
            else if (value is string)
                return FromString((string)value);
            else if (value is LineWidth)
                return new LineThickness((LineWidth)value);
            else if (IsTypeNumeric(value.GetType()))
                return new LineThickness(FixWidth(ToEnum<LineWidth>(value)));
            return base.ConvertFrom(context, culture, value);
        }

        private static LineThickness FromString (string str)
        {
            string[] parts = SplitNumbers(str, 4);
            switch (parts.Length) {
                case 1:
                    return new LineThickness(GetWidth(parts[0]));
                case 2:
                    return new LineThickness(GetWidth(parts[0]), GetWidth(parts[1]));
                case 4:
                    return new LineThickness(GetWidth(parts[0]), GetWidth(parts[1]), GetWidth(parts[2]), GetWidth(parts[3]));
                default:
                    throw new FormatException($"Invalid {nameof(LineThickness)} format: '{str}'.");
            }
        }

        private static LineWidth FixWidth (LineWidth width) => width == LineWidth.None || width == LineWidth.Single ? width : LineWidth.Wide;
        private static LineWidth GetWidth (string str) => FixWidth(ParseEnum<LineWidth>(str));
    }
}