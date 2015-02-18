using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat.Markup
{
    using ConverterDelegate = Func<Object, Object, CultureInfo, Object>;

    [MarkupExtensionReturnType (typeof(object))]
    public class GetExtension : GetExtensionBase
    {
        public string Format { get; set; }
        public ConverterDelegate Converter { get; set; }
        public object Parameter { get; set; }
        public CultureInfo Culture { get; set; }

        public GetExtension ()
        {}

        public GetExtension (string path) : base(path)
        {}

        protected override object ProvideExpression (IServiceProvider provider, Element obj, PropertyInfo prop)
        {
            var expression = new GetExpression {
                Source = Source,
                Path = Path,
                Format = Format != null && Format.IndexOf('{') < 0 ? "{0:" + Format + "}" : Format,
                Converter = Converter,
                Culture = Culture,
                Parameter = Parameter,
                TargetObject = obj,
                TargetType = prop.PropertyType,
            };
            if (obj != null)
                obj.Bind(prop, expression);
            return expression.GetValue();
        }
    }
}