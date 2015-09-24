namespace Alba.CsConsoleFormat.Generation
{
    public class DocumentBuilder
    {
        public static ElementBuilder<T> Create<T> (string text = null)
            where T : Element, new()
        {
            var element = new T();
            if (text != null)
                element.Children.Add(text);
            return new ElementBuilder<T>(element);
        }

        public static ElementBuilder<Span> CreateText (string text = "")
        {
            return new ElementBuilder<Span>(new Span(text ?? ""));
        }
    }
}