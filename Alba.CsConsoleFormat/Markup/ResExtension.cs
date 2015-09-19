using System;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType (typeof(object))]
    public class ResExtension : MarkupExtension
    {
        [ConstructorArgument ("key")]
        public string Key { get; set; }

        public ResExtension (string key)
        {
            Key = key;
        }

        public ResExtension () : this(null)
        {}

        public override object ProvideValue (IServiceProvider provider)
        {
            var rootObjectProvider = provider.GetService<IRootObjectProvider>();
            var doc = (Document)rootObjectProvider.RootObject;
            object value;
            if (doc.Resources.TryGetValue(Key, out value))
                return value;

            var lineInfo = provider.GetService<IXamlLineInfo>();
            throw new InvalidOperationException($"Resource '{Key}' not found ({lineInfo.LineNumber}:{lineInfo.LinePosition}).");
        }
    }
}