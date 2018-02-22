using System;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.ColorfulConsole
{
    [TypeConverter(typeof(FigletGradientConverter))]
    public class FigletGradient
    {
        public FigletGradientStopCollection GradientStops { get; }

        public FigletGradient()
        {
            GradientStops = new FigletGradientStopCollection(this);
        }

        public FigletGradient([CanBeNull] FigletGradientStopCollection gradientStops)
        {
            GradientStops = gradientStops ?? throw new ArgumentNullException(nameof(gradientStops));
        }

        internal ConsoleColor? GetColor(int offset)
        {
            return GradientStops.LastOrDefault(s => s.Offset <= offset)?.Color;
        }
    }
}