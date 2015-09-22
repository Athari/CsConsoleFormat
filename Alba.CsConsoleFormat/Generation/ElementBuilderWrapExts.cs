namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderWrapExts
    {
        public static ElementBuilder<Wrap> CreateWrap (this DocumentBuilder @this,
            Orientation orientation = Orientation.Horizontal, int? itemWidth = null, int? itemHeight = null)
        {
            return new ElementBuilder<Wrap>(new Wrap {
                Orientation = orientation,
                ItemWidth = itemWidth,
                ItemHeight = itemHeight,
            });
        }
    }
}