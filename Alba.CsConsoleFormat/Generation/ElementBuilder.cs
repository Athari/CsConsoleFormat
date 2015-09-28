using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Generation
{
    public sealed class ElementBuilder<T> : IElementBuilder<T>
        where T : Element, new()
    {
        public T Element { get; }

        public ElementBuilder ([NotNull] T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            Element = element;
        }

        Element IElementBuilder.Element => Element;
        Type IElementBuilder.ElementType => typeof(T);

        [SuppressMessage ("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Constructor and Element property are enough.")]
        public static implicit operator T (ElementBuilder<T> @this) => @this?.Element;
    }
}