using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderBorderExts
    {
        public static ElementBuilder<T> Stroke<T> (this ElementBuilder<T> @this, LineThickness stroke)
            where T : Border, new()
        {
            @this.Element.Stroke = stroke;
            return @this;
        }

        public static ElementBuilder<T> Stroke<T> (this ElementBuilder<T> @this, LineWidth left, LineWidth top, LineWidth right, LineWidth bottom)
            where T : Border, new()
        {
            @this.Element.Stroke = new LineThickness(left, top, right, bottom);
            return @this;
        }

        public static ElementBuilder<T> Stroke<T> (this ElementBuilder<T> @this, LineWidth vertical, LineWidth horizontal)
            where T : Border, new()
        {
            @this.Element.Stroke = new LineThickness(vertical, horizontal);
            return @this;
        }

        public static ElementBuilder<T> Stroke<T> (this ElementBuilder<T> @this, LineWidth width)
            where T : Border, new()
        {
            @this.Element.Stroke = new LineThickness(width);
            return @this;
        }

        public static ElementBuilder<T> Shadow<T> (this ElementBuilder<T> @this, Thickness shadow, ConsoleColor? shadowColor = null,
            [ValueProvider (ValueProviders.ColorMaps)] ConsoleColor[] shadowColorMap = null)
            where T : Border, new()
        {
            @this.Element.Shadow = shadow;
            if (shadowColor != null)
                @this.Element.ShadowColor = shadowColor;
            if (shadowColorMap != null)
                @this.Element.ShadowColorMap = shadowColorMap;
            return @this;
        }
    }
}