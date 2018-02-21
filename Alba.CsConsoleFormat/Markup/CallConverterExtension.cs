using System;
using System.Globalization;
using System.Reflection;
using Alba.CsConsoleFormat.Framework.Sys;
#if SYSTEM_XAML
using System.Windows.Markup;
using System.Xaml;
#elif PORTABLE_XAML
using Portable.Xaml;
using Portable.Xaml.Markup;
#endif

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType(typeof(ConverterFunc))]
    public class CallConverterExtension : GetExtensionBase
    {
        public CultureInfo Culture { get; set; }

        public CallConverterExtension()
        { }

        public CallConverterExtension(string path) : base(path)
        { }

        protected override object ProvideExpression(IServiceProvider provider, BindableObject obj, PropertyInfo property)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            var expression = new CallConverterExpression {
                Source = Source,
                Path = Path,
                Culture = Culture,
            };
            return expression.GetValue(obj ?? provider.GetService<IRootObjectProvider>().RootObject);
        }
    }
}