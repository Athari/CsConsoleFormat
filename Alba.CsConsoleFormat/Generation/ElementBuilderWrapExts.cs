namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderWrapExts
    {
        public static ElementBuilder<T> Orient<T> (this ElementBuilder<T> @this, Orientation orientation)
            where T : Wrap, new()
        {
            @this.Element.Orientation = orientation;
            return @this;
        }

        public static ElementBuilder<T> SizeItems<T> (this ElementBuilder<T> @this, int? itemWidth = null, int? itemHeight = null)
            where T : Wrap, new()
        {
            @this.Element.ItemWidth = itemWidth;
            @this.Element.ItemHeight = itemHeight;
            return @this;
        }
    }
}