using System;
using System.Reflection;
using Alba.CsConsoleFormat.Framework.Text;

// TODO Support more complex getter expressions
// TODO Support converters properly (see MS.Internal.Data.DefaultValueConverter)
namespace Alba.CsConsoleFormat.Markup
{
    public class GetExpression
    {
        public object Source { get; set; }
        public string Path { get; set; }
        public Func<object, object> Converter { get; set; }
        public Type TargetType { get; set; }

        public object GetValue (object dataContext)
        {
            object source = Source ?? dataContext;
            if (source == null)
                return null;
            if (Path.IsNullOrEmpty())
                return ConvertValue(source);

            object value = source;
            foreach (string propName in Path.Split('.')) {
                if (value == null)
                    return ConvertValue(null);
                PropertyInfo prop = value.GetType().GetProperty(propName);
                if (prop == null)
                    throw new InvalidOperationException("Cannot resolve property '{0}'.".Fmt(propName));
                value = prop.GetValue(value);
            }
            return ConvertValue(value);
        }

        private object ConvertValue (object value)
        {
            if (Converter != null)
                value = Converter(value);
            if (value == null) // TODO ???
                return null;
            Type valueType = value.GetType();
            if (TargetType == valueType || TargetType.IsAssignableFrom(valueType))
                return value;
            return Convert.ChangeType(value, TargetType);
        }
    }
}