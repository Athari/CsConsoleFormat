using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage ("ReSharper", "RedundantExplicitArraySize", Justification = "Provides validation of array size.")]
    public static class ColorMaps
    {
        internal const int ConsoleColorCount = 16;

        public static readonly IList<ConsoleColor> Dark = Array.AsReadOnly(new ConsoleColor[ConsoleColorCount] {
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, DarkGray,
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray,
        });
        public static readonly IList<ConsoleColor> Darkest = Array.AsReadOnly(new ConsoleColor[ConsoleColorCount] {
            Black, Black, Black, Black, Black, Black, Black, DarkGray,
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray,
        });
        public static readonly IList<ConsoleColor> Light = Array.AsReadOnly(new ConsoleColor[ConsoleColorCount] {
            DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
            Gray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
        });
        public static readonly IList<ConsoleColor> Lightest = Array.AsReadOnly(new ConsoleColor[ConsoleColorCount] {
            DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
            Gray, White, White, White, White, White, White, White,
        });
        public static readonly IList<ConsoleColor> Invert = Array.AsReadOnly(new ConsoleColor[ConsoleColorCount] {
            White, DarkYellow, DarkMagenta, DarkRed, DarkCyan, DarkGreen, DarkBlue, DarkGray,
            Green, Yellow, Magenta, Red, Cyan, Green, Blue, Black,
        });
    }
}