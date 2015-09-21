namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderSpanExts
    {
        public static ElementBuilder<Span> CreateText (this DocumentBuilder @this, string text)
        {
            return new ElementBuilder<Span>(new Span(text ?? ""));
        }

        public static ElementBuilder<Span> CreateText (this DocumentBuilder @this, out Span element, string text)
        {
            return new ElementBuilder<Span>(element = new Span(text ?? ""));
        }
    }
}