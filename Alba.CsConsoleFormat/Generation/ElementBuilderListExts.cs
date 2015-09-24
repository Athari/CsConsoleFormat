namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderListExts
    {
        public static ElementBuilder<T> Index<T> (this ElementBuilder<T> @this,
            string indexFormat = List.DefaultIndexFormat, int startIndex = 1)
            where T : List, new()
        {
            @this.Element.IndexFormat = indexFormat;
            @this.Element.StartIndex = startIndex;
            return @this;
        }
    }
}