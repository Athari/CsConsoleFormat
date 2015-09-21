namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderStackExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this,
            Orientation orientation = Orientation.Vertical)
            where T : Stack, new()
        {
            return new ElementBuilder<T>(new T { Orientation = orientation });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element,
            Orientation orientation = Orientation.Vertical)
            where T : Stack, new()
        {
            return new ElementBuilder<T>(element = new T { Orientation = orientation });
        }
    }
}