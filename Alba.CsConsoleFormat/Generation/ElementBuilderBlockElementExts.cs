namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderBlockElementExts
    {
        public static ElementBuilder<T> Size<T> (this ElementBuilder<T> @this, int? width = null, int? height = null)
            where T : BlockElement, new()
        {
            if (width != null)
                @this.Element.Width = width.Value;
            if (height != null)
                @this.Element.Height = height.Value;
            return @this;
        }

        public static ElementBuilder<T> LimitSize<T> (this ElementBuilder<T> @this,
            int? minWidth = null, int? minHeight = null, int? maxWidth = null, int? maxHeight = null)
            where T : BlockElement, new()
        {
            if (minWidth != null)
                @this.Element.MinWidth = minWidth.Value;
            if (minHeight != null)
                @this.Element.MinHeight = minHeight.Value;
            if (maxWidth != null)
                @this.Element.MaxWidth = maxWidth.Value;
            if (maxHeight != null)
                @this.Element.MaxHeight = maxHeight.Value;
            return @this;
        }

        public static ElementBuilder<T> Align<T> (this ElementBuilder<T> @this, HorizontalAlignment align)
            where T : BlockElement, new()
        {
            @this.Element.Align = align;
            return @this;
        }

        public static ElementBuilder<T> Align<T> (this ElementBuilder<T> @this, VerticalAlignment valign)
            where T : BlockElement, new()
        {
            @this.Element.VAlign = valign;
            return @this;
        }

        public static ElementBuilder<T> AlignText<T> (this ElementBuilder<T> @this, TextAlignment align)
            where T : BlockElement, new()
        {
            @this.Element.TextAlign = align;
            return @this;
        }

        public static ElementBuilder<T> WrapText<T> (this ElementBuilder<T> @this, TextWrapping wrap)
            where T : BlockElement, new()
        {
            @this.Element.TextWrap = wrap;
            return @this;
        }

        public static ElementBuilder<T> Margin<T> (this ElementBuilder<T> @this, Thickness margin)
            where T : BlockElement, new()
        {
            @this.Element.Margin = margin;
            return @this;
        }

        public static ElementBuilder<T> Margin<T> (this ElementBuilder<T> @this, int left, int top, int right, int bottom)
            where T : BlockElement, new()
        {
            @this.Element.Margin = new Thickness(left, top, right, bottom);
            return @this;
        }

        public static ElementBuilder<T> Margin<T> (this ElementBuilder<T> @this, int vertical, int horizontal)
            where T : BlockElement, new()
        {
            @this.Element.Margin = new Thickness(vertical, horizontal);
            return @this;
        }

        public static ElementBuilder<T> Margin<T> (this ElementBuilder<T> @this, int width)
            where T : BlockElement, new()
        {
            @this.Element.Margin = new Thickness(width);
            return @this;
        }
    }
}