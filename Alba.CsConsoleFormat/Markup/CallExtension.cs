using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat.Markup
{
    using ConverterDelegate = Func<Object, Object, CultureInfo, Object>;

    [MarkupExtensionReturnType (typeof(ConverterDelegate))]
    public class CallExtension : GetExtensionBase
    {
        public CultureInfo Culture { get; set; }

        public CallExtension ()
        {}

        public CallExtension (string path) : base(path)
        {}

        protected override object ProvideExpression (IServiceProvider provider, Element obj, PropertyInfo prop)
        {
            var expression = new CallExpression {
                Source = Source,
                Path = Path,
                Culture = Culture,
            };
            return expression.GetValue(obj ?? provider.GetService<IRootObjectProvider>().RootObject);
        }
    }
}