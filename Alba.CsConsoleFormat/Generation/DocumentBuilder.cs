namespace Alba.CsConsoleFormat.Generation
{
    public class DocumentBuilder
    {
        public static ElementBuilder<T> Create<T> (params object[] children)
            where T : Element, new()
        {
            return new ElementBuilder<T>(new T()).AddChildren(children);
        }

        public static ElementBuilder<Span> CreateText (string text = "")
        {
            return new ElementBuilder<Span>(new Span(text ?? ""));
        }
    }
}