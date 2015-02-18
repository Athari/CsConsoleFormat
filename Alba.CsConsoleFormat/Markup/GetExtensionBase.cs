using System;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat.Markup
{
    public abstract class GetExtensionBase : MarkupExtension
    {
        [ConstructorArgument ("path")]
        public string Path { get; set; }
        public string Element { get; set; }
        public object Source { get; set; }

        protected GetExtensionBase (string path)
        {
            Path = path;
        }

        protected GetExtensionBase () : this(null)
        {}

        public override sealed object ProvideValue (IServiceProvider provider)
        {
            //var xamlSchemaContextProvider = provider.GetService<IXamlSchemaContextProvider>();
            //var xamlTypeResolver = provider.GetService<IXamlTypeResolver>();
            //var xamlNamespaceResolver = provider.GetService<IXamlNamespaceResolver>();
            //var xamlObjectWriterFactory = provider.GetService<IXamlObjectWriterFactory>();
            //var destinationTypeProvider = provider.GetService<IDestinationTypeProvider>();
            //var uriContext = provider.GetService<IUriContext>();
            //var rootObjectProvider = provider.GetService<IRootObjectProvider>();
            //var ambientProvider = provider.GetService<IAmbientProvider>();

            if (Element != null) {
                var nameResolver = provider.GetService<IXamlNameResolver>();
                object element = nameResolver.Resolve(Element);
                if (element != null)
                    Source = element;
                else if (nameResolver.IsFixupTokenAvailable)
                    return nameResolver.GetFixupToken(new[] { Element });
                else {
                    var lineInfo = provider.GetService<IXamlLineInfo>();
                    throw new InvalidOperationException("Element '{0}' not found ({1}:{2})."
                        .Fmt(Element, lineInfo.LineNumber, lineInfo.LinePosition));
                }
            }

            var targetProvider = provider.GetService<IProvideValueTarget>();
            var obj = targetProvider.TargetObject as Element;
            var prop = (PropertyInfo)targetProvider.TargetProperty;

            return ProvideExpression(provider, obj, prop);
        }

        protected abstract object ProvideExpression (IServiceProvider provider, Element obj, PropertyInfo prop);

        public override string ToString ()
        {
            return "{{Get Path={0}}}".FmtInv(Path);
        }
    }
}