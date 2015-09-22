namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderSeparatorExts
    {
        public static ElementBuilder<Separator> CreateSeparator (this DocumentBuilder @this,
            Orientation orientation = Orientation.Horizontal, LineWidth stroke = LineWidth.Single)
        {
            return new ElementBuilder<Separator>(new Separator {
                Orientation = orientation,
                Stroke = stroke,
            });
        }
    }
}