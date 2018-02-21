using System;
using System.Reflection;
using Alba.CsConsoleFormat.Framework.Sys;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Windows.Markup;
using System.Xaml;
#elif PORTABLE_XAML
using Portable.Xaml;
using Portable.Xaml.Markup;
#endif

namespace Alba.CsConsoleFormat.Markup
{
    public abstract class GetExtensionBase : MarkupExtension
    {
        [CanBeNull, ConstructorArgument("path")]
        public string Path { get; set; }

        [CanBeNull]
        public string Element { get; set; }

        [CanBeNull]
        public object Source { get; set; }

        protected GetExtensionBase(string path)
        {
            Path = path;
        }

        protected GetExtensionBase() : this(null)
        { }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
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
            var property = (PropertyInfo)targetProvider.TargetProperty;

            return ProvideExpression(serviceProvider, obj, property);
        }

        [CanBeNull]
        protected abstract object ProvideExpression([NotNull] IServiceProvider provider, [CanBeNull] BindableObject obj, [CanBeNull] PropertyInfo property);

        public override string ToString() => $"{{Get Path={Path}}}";
    }
}