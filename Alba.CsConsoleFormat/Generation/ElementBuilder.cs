namespace Alba.CsConsoleFormat.Generation
{
    public abstract class ElementBuilder
    {
        internal abstract Element ElementUntyped { get; }

        internal ElementBuilder ()
        {}
    }

    public class ElementBuilder<T> : ElementBuilder
        where T : Element, new()
    {
        public T Element { get; }

        internal ElementBuilder (T element)
        {
            Element = element;
        }

        internal override Element ElementUntyped => Element;

        public static implicit operator T (ElementBuilder<T> @this) => @this.Element;
    }
}