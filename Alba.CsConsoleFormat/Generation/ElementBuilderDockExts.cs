namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderDockExts
    {
        public static ElementBuilder<T> Config<T> (this ElementBuilder<T> @this, bool lastChildFill)
            where T : Dock, new()
        {
            @this.Element.LastChildFill = lastChildFill;
            return @this;
        }

        public static ElementBuilder<T> DockTo<T> (this ElementBuilder<T> @this, DockTo to)
            where T : BlockElement, new()
        {
            Dock.SetTo(@this.Element, to);
            return @this;
        }
    }
}