using System;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat.Markup
{
    public abstract class GetExtensionBase : MarkupExtension
    {
        [ConstructorArgument("path")]
        public string Path { get; set; }
        public string Element { get; set; }
        public object Source { get; set; }

        protected GetExtensionBase(string path)
        {
            Path = path;
        }

        protected GetExtensionBase() : this(null)
        {}

        public override sealed object ProvideValue(IServiceProvider serviceProvider)
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
                var nameResolver = serviceProvider.GetService<IXamlNameResolver>();
                object element = nameResolver.Resolve(Element);
                if (element != null)
                    Source = element;
                else if (nameResolver.IsFixupTokenAvailable)
                    return nameResolver.GetFixupToken(new[] { Element });
                else {
                    var lineInfo = serviceProvider.GetService<IXamlLineInfo>();
                    throw new InvalidOperationException($"Element '{Element}' not found ({lineInfo.LineNumber}:{lineInfo.LinePosition}).");
                }
            }

            var targetProvider = serviceProvider.GetService<IProvideValueTarget>();
            var obj = targetProvider.TargetObject as BindableObject;
            var prop = (PropertyInfo)targetProvider.TargetProperty;

            return ProvideExpression(serviceProvider, obj, prop);
        }

        protected abstract object ProvideExpression(IServiceProvider provider, BindableObject obj, PropertyInfo prop);

        public override string ToString() => $"{{Get Path={Path}}}";
    }
}