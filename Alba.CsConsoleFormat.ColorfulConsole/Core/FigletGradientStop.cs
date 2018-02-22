using System;

namespace Alba.CsConsoleFormat.ColorfulConsole
{
    public class FigletGradientStop
    {
        internal const int AutoOffset = -1;

        public ConsoleColor? Color { get; set; }
        public int Offset { get; set; }

        public FigletGradientStop()
        { }

        public FigletGradientStop(ConsoleColor? color, int offset = AutoOffset)
        {
            Color = color;
            Offset = offset;
        }
    }
}