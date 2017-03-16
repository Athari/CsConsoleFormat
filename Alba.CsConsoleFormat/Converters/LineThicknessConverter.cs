using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

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
    public class LineThicknessConverter : SequenceTypeConverter<LineThickness>
    {
        private static readonly Lazy<ConstructorInfo> LineThicknessConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(LineThickness).GetConstructor(new[] { typeof(LineWidth), typeof(LineWidth), typeof(LineWidth), typeof(LineWidth) }));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || IsTypeNumeric(sourceType) || sourceType == typeof(LineWidth);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case LineWidth width:
                    return new LineThickness(width);
                case object number when number.IsTypeNumeric():
                    return new LineThickness(FixWidth(NumberToEnum<LineWidth>(number)));
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        protected override LineThickness FromString(string str)
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

        protected override ConstructorInfo InstanceConstructor => LineThicknessConstructor.Value;
        protected override object[] InstanceConstructorArgs(LineThickness o) => new object[] { o.Left, o.Top, o.Right, o.Bottom };

        private static LineWidth FixWidth(LineWidth width) => width == LineWidth.None || width == LineWidth.Single ? width : LineWidth.Wide;
        private static LineWidth GetWidth(string str) => FixWidth(StringToEnum<LineWidth>(str));
    }
}