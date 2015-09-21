namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderColumnsExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, GridLength width)
            where T : Column, new()
        {
            return new ElementBuilder<T>(new T { Width = width });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element, GridLength width)
            where T : Column, new()
        {
            return new ElementBuilder<T>(element = new T { Width = width });
        }

        public static ElementBuilder<T> LimitSize<T> (this ElementBuilder<T> @this, int? minWidth = null, int? maxWidth = null)
            where T : Column, new()
        {
            if (minWidth != null)
                @this.Element.MinWidth = minWidth.Value;
            if (maxWidth != null)
                @this.Element.MaxWidth = maxWidth.Value;
            return @this;
        }
    }
}