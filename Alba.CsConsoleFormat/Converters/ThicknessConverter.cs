using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Thickness"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"1 2 3 4" - <c>new Thickness(1, 2, 3, 4)</c></item>
    /// <item>"1 2" - <c>new Thickness(1, 2)</c> (<c>new Thickness(1, 2, 1, 2)</c>)</item>
    /// <item>"1", 1 - <c>new Thickness(1)</c> (<c>new Thickness(1, 1, 1, 1)</c>)</item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class ThicknessConverter : SequenceTypeConverter<Thickness>
    {
        private static readonly Lazy<ConstructorInfo> ThicknessConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Thickness).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int) }));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || IsTypeNumeric(sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case object number when number.IsTypeNumeric():
                    return new Thickness(ToInt(number));
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        protected override Thickness FromString(string str)
        {
            string[] parts = SplitNumbers(str, 4);
            switch (parts.Length) {
                case 1:
                    return new Thickness(ParseInt(parts[0]));
                case 2:
                    return new Thickness(ParseInt(parts[0]), ParseInt(parts[1]));
                case 4:
                    return new Thickness(ParseInt(parts[0]), ParseInt(parts[1]), ParseInt(parts[2]), ParseInt(parts[3]));
                default:
                    throw new FormatException($"Invalid Thickness format: '{str}'.");
            }
        }

        protected override ConstructorInfo InstanceConstructor => ThicknessConstructor.Value;
        protected override object[] InstanceConstructorArgs(Thickness o) => new object[] { o.Left, o.Top, o.Right, o.Bottom };
    }
}