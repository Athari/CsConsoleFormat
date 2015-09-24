namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderStackExts
    {
        public static ElementBuilder<T> Orient<T> (this ElementBuilder<T> @this, Orientation orientation)
            where T : Stack, new()
        {
            @this.Element.Orientation = orientation;
            return @this;
        }
    }
}