namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderSeparatorExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this,
            Orientation orientation = Orientation.Horizontal, LineWidth stroke = LineWidth.Single)
            where T : Separator, new()
        {
            return new ElementBuilder<T>(new T { Orientation = orientation, Stroke = stroke });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element,
            Orientation orientation = Orientation.Horizontal, LineWidth stroke = LineWidth.Single)
            where T : Separator, new()
        {
            return new ElementBuilder<T>(element = new T { Orientation = orientation, Stroke = stroke });
        }
    }
}