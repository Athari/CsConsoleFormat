using System;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat.Markup
{
    public class ResExtension : MarkupExtension
    {
        [ConstructorArgument ("name")]
        public string Name { get; set; }

        public ResExtension (string name)
        {
            Name = name;
        }

        public ResExtension () : this(null)
        {}

        public override object ProvideValue (IServiceProvider provider)
        {
            var rootObjectProvider = provider.GetService<IRootObjectProvider>();
            var doc = (Document)rootObjectProvider.RootObject;
            object value;
            if (doc.Resources.TryGetValue(Name, out value))
                return value;

            var lineInfo = provider.GetService<IXamlLineInfo>();
            throw new InvalidOperationException("Resource '{0}' not found ({1}:{2})."
                .Fmt(Name, lineInfo.LineNumber, lineInfo.LinePosition));
        }
    }
}