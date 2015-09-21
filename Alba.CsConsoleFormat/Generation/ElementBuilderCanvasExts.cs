namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderCanvasExts
    {
        public static ElementBuilder<T> At<T> (this ElementBuilder<T> @this,
            int? left = null, int? top = null, int? right = null, int? bottom = null)
            where T : BlockElement, new()
        {
            Canvas.SetLeft(@this.Element, left);
            Canvas.SetTop(@this.Element, top);
            Canvas.SetRight(@this.Element, right);
            Canvas.SetBottom(@this.Element, bottom);
            return @this;
        }
    }
}