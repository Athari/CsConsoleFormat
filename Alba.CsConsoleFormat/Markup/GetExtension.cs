using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType (typeof(object))]
    public class GetExtension : GetExtensionBase
    {
        public string Format { get; set; }
        public Func<object, CultureInfo, object> Converter { get; set; }
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
                TargetObject = obj,
                TargetType = prop.PropertyType,
            };
            if (obj != null)
                obj.Bind(prop, expression);
            return expression.GetValue();
        }
    }
}