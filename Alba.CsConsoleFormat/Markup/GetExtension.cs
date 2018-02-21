using System;
using System.Globalization;
using System.Reflection;
#if SYSTEM_XAML
using System.Windows.Markup;
#elif PORTABLE_XAML
using Portable.Xaml.Markup;
#endif

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType(typeof(object))]
    public class GetExtension : GetExtensionBase
    {
        public string Format { get; set; }
        public ConverterFunc Converter { get; set; }
        public object Parameter { get; set; }
        public CultureInfo Culture { get; set; }

        public GetExtension()
        { }

        public GetExtension(string path) : base(path)
        { }

        protected override object ProvideExpression(IServiceProvider provider, BindableObject obj, PropertyInfo property)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var expression = new GetExpression {
                Source = Source,
                Path = Path,
                Format = Format != null && Format.IndexOf('{') < 0 ? "{0:" + Format + "}" : Format,
                Converter = Converter,
                Culture = Culture,
                Parameter = Parameter,
                TargetObject = obj,
                TargetType = property.PropertyType,
            };
            obj?.Bind(property, expression);
            return expression.GetValue();
        }
    }
}