namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderColumnExts
    {
        public static ElementBuilder<Column> CreateColumn (this DocumentBuilder @this, GridLength? width = null)
        {
            return new ElementBuilder<Column>(new Column {
                Width = width ?? GridLength.Star(1)
            });
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