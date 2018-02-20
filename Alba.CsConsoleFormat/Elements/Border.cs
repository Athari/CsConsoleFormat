using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class Border : Div
    {
        public Thickness Shadow { get; set; }

        public ConsoleColor? ShadowColor { get; set; }

        [CanBeNull, ValueProvider(ValueProviders.ColorMaps)]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Designed to be assigned one of predefined read-only collections.")]
        public IList<ConsoleColor> ShadowColorMap { get; set; }

        public LineThickness Stroke { get; set; }

        public Border()
        {
            Stroke = LineThickness.None;
            ShadowColorMap = ColorMaps.Darkest;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            BlockElement child = VisualChild;
            Size borderThickness = (Stroke.CharThickness + Padding + Thickness.Max(Shadow, 0)).CollapsedThickness;
            if (child != null) {
                child.Measure(availableSize - borderThickness);
                return child.DesiredSize + borderThickness;
            }
            return borderThickness;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            VisualChild?.Arrange(new Rect(finalSize).Deflate(Stroke.CharThickness + Padding + Thickness.Max(Shadow, 0)));
            return finalSize;
        }

        public override void Render(ConsoleBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            Rect renderRectWithoutShadow = new Rect(RenderSize).Deflate(Thickness.Max(Shadow, 0));

            //base.Render(buffer);
            if (Background != null)
                buffer.FillBackgroundRectangle(renderRectWithoutShadow, Background.Value);
            buffer.FillForegroundRectangle(new Rect(RenderSize), EffectiveColor);

            if (!Shadow.IsEmpty) {
                // 3 2 2 1:     -1 -1 2 3:
                // ▄▄▄▄▄▄▄▄▄    oooo▄▄
                // █████████    oooo██
                // ███oooo██     █████
                // ███oooo██     █████
                // ▀▀▀▀▀▀▀▀▀     ▀▀▀▀▀
                Thickness shadowLineDelta = new Thickness(0, 1);
                Thickness shadowOffset = Thickness.Max(-Shadow - shadowLineDelta, 0);
                Rect shadowRect = new Rect(RenderSize).Deflate(shadowOffset);

                if (Shadow.Top != 0)
                    buffer.FillForegroundLine(shadowRect.TopLine, ShadowColor, Chars.LowerHalfBlock);
                if (Shadow.Bottom != 0)
                    buffer.FillForegroundLine(shadowRect.BottomLine, ShadowColor, Chars.UpperHalfBlock);
                buffer.FillForegroundRectangle(shadowRect.Deflate(shadowLineDelta), ShadowColor, Chars.FullBlock);
                if (ShadowColor == null && ShadowColorMap != null)
                    buffer.ApplyColorMap(shadowRect, ShadowColorMap,
                        (ref ConsoleChar c) => c.ForegroundColor = ShadowColorMap[(int)c.BackgroundColor]);
            }
            buffer.FillForegroundRectangle(renderRectWithoutShadow, EffectiveColor);
            buffer.DrawRectangle(renderRectWithoutShadow, EffectiveColor, Stroke);
        }
    }
}