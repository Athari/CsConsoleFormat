using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using static System.FormattableString;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="GridLength"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"Auto" - <c>GridLength.Auto</c></item>
    /// <item>"*" - <c>GridLength.Star(1)</c></item>
    /// <item>"2*" - <c>GridLength.Star(2)</c></item>
    /// <item>"3", 3 - <c>GridLength.Char(3)</c></item>
    /// </list> 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class GridLengthConverter : TypeConverter
    {
        private static readonly Lazy<ConstructorInfo> GridLengthConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(GridLength).GetConstructor(new[] { typeof(int), typeof(GridUnit) }));

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || IsTypeStringOrNumeric(sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            base.CanConvertTo(context, destinationType) || destinationType == typeof(InstanceDescriptor);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case string str:
                    return FromString(str);
                case object number when number.IsTypeNumeric():
                    return GridLength.Char(ToInt(number));
                default:
                    return base.ConvertFrom(context, culture, value);
            }

            GridLength FromString(string str)
            {
                if (str.ToUpperInvariant() == AUTO)
                    return new GridLength(0, GridUnit.Auto);
                else if (str.EndsWith(Asterisk, StringComparison.Ordinal))
                    return new GridLength(str.Length > 1 ? ParseInt(str.Remove(str.Length - 1)) : 1, GridUnit.Star);
                else
                    return new GridLength(ParseInt(str), GridUnit.Char);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, [CanBeNull] object value, Type destinationType)
        {
            if (!(value is GridLength length))
                throw GetConvertToException(value, destinationType);

            if (destinationType == typeof(string))
                return ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(GridLengthConstructor.Value, new object[] { length.Value, length.Unit }, true);
            else
                return base.ConvertTo(context, culture, value, destinationType);

            string ToString()
            {
                switch (length.Unit) {
                    case GridUnit.Auto:
                        return Auto;
                    case GridUnit.Char:
                        return Invariant($"{length.Value}");
                    case GridUnit.Star:
                        return length.Value > 1 ? Invariant($"{length.Value}{Asterisk}") : Asterisk;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}