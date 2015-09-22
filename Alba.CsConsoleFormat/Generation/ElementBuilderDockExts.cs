namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderDockExts
    {
        public static ElementBuilder<Dock> CreateDock (this DocumentBuilder @this, bool lastChildFill = true)
        {
            return new ElementBuilder<Dock>(new Dock {
                LastChildFill = lastChildFill
            });
        }

        public static ElementBuilder<T> DockTo<T> (this ElementBuilder<T> @this, DockTo to)
            where T : BlockElement, new()
        {
            Dock.SetTo(@this.Element, to);
            return @this;
        }
    }
}