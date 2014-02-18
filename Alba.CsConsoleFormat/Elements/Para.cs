using System;

namespace Alba.CsConsoleFormat
{
    public class Para : Container
    {
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? BgColor { get; set; }
        public HorizontalAlignment Align { get; set; }
        public VerticalAlignment VAlign { get; set; }
    }
}