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