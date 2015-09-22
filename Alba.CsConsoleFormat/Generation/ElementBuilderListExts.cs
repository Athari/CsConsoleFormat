namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderListExts
    {
        public static ElementBuilder<List> CreateList (this DocumentBuilder @this,
            string indexFormat = List.DefaultIndexFormat, int startIndex = 1)
        {
            return new ElementBuilder<List>(new List {
                IndexFormat = indexFormat,
                StartIndex = startIndex,
            });
        }
    }
}