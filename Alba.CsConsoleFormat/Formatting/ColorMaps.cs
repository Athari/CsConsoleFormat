using System;

// ReSharper disable RedundantExplicitArraySize
namespace Alba.CsConsoleFormat
{
    using C = ConsoleColor;

    public static class ColorMaps
    {
        internal const int ConsoleColorCount = 16;

        public static readonly C[] Dark = new C[ConsoleColorCount] {
            C.Black, C.DarkBlue, C.DarkGreen, C.DarkCyan, C.DarkRed, C.DarkMagenta, C.DarkYellow, C.DarkGray,
            C.Black, C.DarkBlue, C.DarkGreen, C.DarkCyan, C.DarkRed, C.DarkMagenta, C.DarkYellow, C.Gray,
        };
        public static readonly C[] Darkest = new C[ConsoleColorCount] {
            C.Black, C.Black, C.Black, C.Black, C.Black, C.Black, C.Black, C.DarkGray,
            C.Black, C.DarkBlue, C.DarkGreen, C.DarkCyan, C.DarkRed, C.DarkMagenta, C.DarkYellow, C.Gray,
        };
        public static readonly C[] Light = new C[ConsoleColorCount] {
            C.DarkGray, C.Blue, C.Green, C.Cyan, C.Red, C.Magenta, C.Yellow, C.White,
            C.Gray, C.Blue, C.Green, C.Cyan, C.Red, C.Magenta, C.Yellow, C.White,
        };
        public static readonly C[] Lightest = new C[ConsoleColorCount] {
            C.DarkGray, C.Blue, C.Green, C.Cyan, C.Red, C.Magenta, C.Yellow, C.White,
            C.Gray, C.White, C.White, C.White, C.White, C.White, C.White, C.White,
        };
        public static readonly C[] Invert = new C[ConsoleColorCount] {
            C.White, C.DarkYellow, C.DarkMagenta, C.DarkRed, C.DarkCyan, C.DarkGreen, C.DarkBlue, C.DarkGray,
            C.Green, C.Yellow, C.Magenta, C.Red, C.Cyan, C.Green, C.Blue, C.Black,
        };
    }
}