using System;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Sys;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat.Markup
{
    [MarkupExtensionReturnType (typeof(object))]
    public class GetExtension : MarkupExtension
    {
        public string Path { get; set; }
        public string Element { get; set; }
        public object Source { get; set; }

        public GetExtension (string path)
        {
            Path = path;
        }

        public GetExtension () : this(null)
        {}

        public override object ProvideValue (IServiceProvider provider)
        {
            var lineInfo = provider.GetService<IXamlLineInfo>();
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
                else
                    throw new InvalidOperationException("Element '{0}' not found ({1}:{2})."
                        .Fmt(Element, lineInfo.LineNumber, lineInfo.LinePosition));
            }

            var targetProvider = provider.GetService<IProvideValueTarget>();
            var obj = (Element)targetProvider.TargetObject;
            var prop = (PropertyInfo)targetProvider.TargetProperty;

            var expression = new GetExpression {
                Source = Source ?? obj.DataContext,
                Path = Path,
            };
            obj.Getters[prop] = expression;

            return expression.GetValue();
        }
    }
}