using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    public class InlineElement : Element
    {
        [TypeConverter (typeof(ColorConverter))]
        public ConsoleColor? Color { get; set; }

        [TypeConverter (typeof(ColorConverter))]
        public ConsoleColor? BgColor { get; set; }
    }
}