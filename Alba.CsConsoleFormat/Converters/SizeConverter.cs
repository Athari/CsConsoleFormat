using System;
using System.ComponentModel;
using System.Globalization;
using Alba.CsConsoleFormat.Framework.Text;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
namespace Alba.CsConsoleFormat
{
    public class SizeConverter : TypeConverter
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

        private static Size FromString (string str)
        {
            string[] parts = str.Split(new[] { ' ', ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new FormatException("Invalid Size format: \"{0}\"".Fmt(str));
            return new Size(GetValue(parts[0]), GetValue(parts[1]));
        }

        private static int GetValue (string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}