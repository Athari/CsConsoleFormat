namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderStackExts
    {
        public static ElementBuilder<Stack> CreateStack (this DocumentBuilder @this, Orientation orientation = Orientation.Vertical)
        {
            return new ElementBuilder<Stack>(new Stack {
                Orientation = orientation
            });
        }
    }
}