using System;
using System.Linq;
using Colorful;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.ColorfulConsole
{
    public class FigletDiv : Div
    {
        private static readonly Lazy<FigletFont> DefaultFigletFont = new Lazy<FigletFont>(() => FigletFont.Parse(DefaultFonts.SmallSlant));

        private string[] _lines;

        [CanBeNull, CLSCompliant(false)]
        public FigletFont FigletFont { get; set; } = DefaultFigletFont.Value;

        [CanBeNull]
        public string Text { get; set; }

        [CanBeNull]
        public FigletGradient BackgroundGradient { get; set; }

        [CanBeNull]
        public FigletGradient ColorGradient { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (FigletFont == null)
                return Size.Empty;
            _lines = new Figlet(FigletFont).ToAscii(Text ?? "").ConcreteValue.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return new Size(_lines.FirstOrDefault()?.Length ?? 0, _lines.Length - 1);
        }

        protected override void RenderOverride(ConsoleBuffer buffer)
        {
            base.RenderOverride(buffer);
            ConsoleColor color = EffectiveColor, background = EffectiveBackground;
            for (int i = 0; i < _lines.Length - 1; i++) {
                string line = _lines[i];
                buffer.FillBackgroundRectangle(0, i, line.Length, 1, BackgroundGradient?.GetColor(i) ?? background);
                buffer.DrawString(0, i, ColorGradient?.GetColor(i) ?? color, line);
            }
        }

        public override string ToString() => base.ToString() + $" Text=\"{Text}\"";
    }
}