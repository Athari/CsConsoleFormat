using System;

namespace Alba.CsConsoleFormat.Fluent
{
    public static class ConsoleColorExts
    {
        public static ConsoleColorOnBackground On(this ConsoleColor @this, ConsoleColor background) =>
            new ConsoleColorOnBackground(@this, background);

        public struct ConsoleColorOnBackground
        {
            public ConsoleColor Color { get; }
            public ConsoleColor Background { get; }

            public ConsoleColorOnBackground(ConsoleColor color, ConsoleColor background)
            {
                Color = color;
                Background = background;
            }
        }
    }
}