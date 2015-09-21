namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderListExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this,
            string indexFormat = List.DefaultIndexFormat, int startIndex = 1)
            where T : List, new()
        {
            return new ElementBuilder<T>(new T { IndexFormat = indexFormat, StartIndex = startIndex });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element,
            string indexFormat = List.DefaultIndexFormat, int startIndex = 1)
            where T : List, new()
        {
            return new ElementBuilder<T>(element = new T { IndexFormat = indexFormat, StartIndex = startIndex });
        }
    }
}