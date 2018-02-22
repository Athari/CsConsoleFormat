using System.Collections.ObjectModel;
using System.Linq;

namespace Alba.CsConsoleFormat.ColorfulConsole
{
    public class FigletGradientStopCollection : Collection<FigletGradientStop>
    {
        private readonly FigletGradient _gradient;

        public FigletGradientStopCollection(FigletGradient gradient)
        {
            _gradient = gradient;
        }

        protected override void InsertItem(int index, FigletGradientStop item)
        {
            base.InsertItem(index, item);
            if (item.Offset == FigletGradientStop.AutoOffset)
                item.Offset = _gradient.GradientStops.Max(s => s.Offset) + 1;
        }
    }
}