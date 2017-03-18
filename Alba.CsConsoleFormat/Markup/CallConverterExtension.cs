using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType(typeof(ConverterFunc))]
    public class CallConverterExtension : GetExtensionBase
    {
        public CultureInfo Culture { get; set; }

        public CallConverterExtension()
        {}

        public CallConverterExtension(string path) : base(path)
        {}

        protected override object ProvideExpression(IServiceProvider provider, BindableObject obj, PropertyInfo property)
        {
            var expression = new CallConverterExpression {
                Source = Source,
                Path = Path,
                Culture = Culture,
            };
            return expression.GetValue(obj ?? provider.GetService<IRootObjectProvider>().RootObject);
        }
    }
}