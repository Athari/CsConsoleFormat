namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderColumnExts
    {
        public static ElementBuilder<T> Size<T> (this ElementBuilder<T> @this, GridLength width)
            where T : Column, new()
        {
            @this.Element.Width = width;
            return @this;
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