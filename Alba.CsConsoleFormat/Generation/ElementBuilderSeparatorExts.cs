namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderSeparatorExts
    {
        public static ElementBuilder<T> Orient<T> (this ElementBuilder<T> @this,
            Orientation orientation = Orientation.Horizontal, LineWidth stroke = LineWidth.Single)
            where T : Separator, new()
        {
            @this.Element.Orientation = orientation;
            @this.Element.Stroke = stroke;
            return @this;
        }
    }
}