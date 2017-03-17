using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public abstract class SequenceTypeConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            base.CanConvertFrom(context, sourceType) || sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            base.CanConvertTo(context, destinationType) || destinationType == typeof(InstanceDescriptor);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value) {
                case string str:
                    return FromString(str);
                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is T))
                throw GetConvertToException(value, destinationType);

            var obj = (T)value;
            if (destinationType == typeof(string))
                return obj.ToString();
            else if (destinationType == typeof(InstanceDescriptor))
                return new InstanceDescriptor(InstanceConstructor, InstanceConstructorArgs(obj), true);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        protected abstract ConstructorInfo InstanceConstructor { get; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected abstract object[] InstanceConstructorArgs([NotNull] T o);

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected abstract T FromString([NotNull] string str);
    }
}