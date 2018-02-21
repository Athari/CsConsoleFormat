using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.Framework.Collections;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage("ReSharper", "RedundantExplicitArraySize", Justification = "Provides validation of array size.")]
    public static class ColorMaps
    {
        internal const int ConsoleColorCount = 16;

        public static readonly IList<ConsoleColor> Dark = new ConsoleColor[ConsoleColorCount] {
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, DarkGray,
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray,
        }.ToReadOnly();
        public static readonly IList<ConsoleColor> Darkest = new ConsoleColor[ConsoleColorCount] {
            Black, Black, Black, Black, Black, Black, Black, DarkGray,
            Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray,
        }.ToReadOnly();
        public static readonly IList<ConsoleColor> Light = new ConsoleColor[ConsoleColorCount] {
            DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
            Gray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
        }.ToReadOnly();
        public static readonly IList<ConsoleColor> Lightest = new ConsoleColor[ConsoleColorCount] {
            DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White,
            Gray, White, White, White, White, White, White, White,
        }.ToReadOnly();
        public static readonly IList<ConsoleColor> Invert = new ConsoleColor[ConsoleColorCount] {
            White, DarkYellow, DarkMagenta, DarkRed, DarkCyan, DarkGreen, DarkBlue, DarkGray,
            Green, Yellow, Magenta, Red, Cyan, Green, Blue, Black,
        }.ToReadOnly();
    }
}