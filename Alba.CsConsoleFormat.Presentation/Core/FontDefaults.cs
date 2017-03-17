using System.Windows;
using System.Windows.Media;

namespace Alba.CsConsoleFormat.Presentation
{
    internal static class FontDefaults
    {
        public static FontFamily DefaultFontFamily => new FontFamily("Consolas");
        public static double DefaultFontSize => 12;
        public static FontStretch DefaultFontStretch => FontStretches.Normal;
        public static FontStyle DefaultFontStyle => FontStyles.Normal;
        public static FontWeight DefaultFontWeight => FontWeights.Normal;

        public static int DefaultConsoleWidth => 80;
        public static Brush DefaultConsoleBackground => Brushes.Black;
    }
}