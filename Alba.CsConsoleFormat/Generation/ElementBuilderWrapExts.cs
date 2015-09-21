namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderWrapExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this,
            Orientation orientation = Orientation.Horizontal, int? itemWidth = null, int? itemHeight = null)
            where T : Wrap, new()
        {
            return new ElementBuilder<T>(new T { Orientation = orientation, ItemWidth = itemWidth, ItemHeight = itemHeight });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element,
            Orientation orientation = Orientation.Horizontal, int? itemWidth = null, int? itemHeight = null)
            where T : Wrap, new()
        {
            return new ElementBuilder<T>(element = new T { Orientation = orientation, ItemWidth = itemWidth, ItemHeight = itemHeight });
        }
    }
}