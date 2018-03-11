﻿using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="ConsoleColor"/> to and from <see cref="string"/> and numeric types:
    /// <list type="bullet">
    /// <item>"Black", 0 - <c>ConsoleColor.Black</c></item>
    /// <item>"inherit" - <c>null</c></item>
    /// </list> 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class ConsoleColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || IsTypeStringOrNumeric(sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            base.CanConvertTo(context, destinationType) || IsTypeNumeric(destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case object number when number.IsTypeNumeric():
                    return NumberToEnum<ConsoleColor>(number);
                case string inherit when inherit.ToUpperInvariant() == INHERIT:
                    return null;
                case string str:
                    return StringToEnum<ConsoleColor>(str);
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, [CanBeNull] object value, Type destinationType)
        {
            if (destinationType == typeof(string)) {
                switch (value) {
                    case null:
                        return Inherit;
                    case ConsoleColor color:
                        return color.ToString();
                }
            }
            else if (IsTypeNumeric(destinationType)) {
                switch (value) {
                    case null:
                        return null;
                    case ConsoleColor color:
                        return Convert.ChangeType(color, destinationType);
                }
            }
            throw GetConvertToException(value, destinationType);
        }
    }
}