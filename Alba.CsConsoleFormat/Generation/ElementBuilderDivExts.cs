namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderDivExts
    {
        public static ElementBuilder<T> Padding<T> (this ElementBuilder<T> @this, Thickness padding)
            where T : Div, new()
        {
            @this.Element.Padding = padding;
            return @this;
        }

        public static ElementBuilder<T> Padding<T> (this ElementBuilder<T> @this, int left, int top, int right, int bottom)
            where T : Div, new()
        {
            @this.Element.Padding = new Thickness(left, top, right, bottom);
            return @this;
        }

        public static ElementBuilder<T> Padding<T> (this ElementBuilder<T> @this, int vertical, int horizontal)
            where T : Div, new()
        {
            @this.Element.Padding = new Thickness(vertical, horizontal);
            return @this;
        }

        public static ElementBuilder<T> Padding<T> (this ElementBuilder<T> @this, int width)
            where T : Div, new()
        {
            @this.Element.Padding = new Thickness(width);
            return @this;
        }
    }
}