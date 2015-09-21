namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderDockExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, bool lastChildFill = true)
            where T : Dock, new()
        {
            return new ElementBuilder<T>(new T { LastChildFill = lastChildFill });
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element, bool lastChildFill = true)
            where T : Dock, new()
        {
            return new ElementBuilder<T>(element = new T { LastChildFill = lastChildFill });
        }

        public static ElementBuilder<T> DockTo<T> (this ElementBuilder<T> @this, DockTo to)
            where T : BlockElement, new()
        {
            Dock.SetTo(@this.Element, to);
            return @this;
        }
    }
}