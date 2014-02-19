using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    public abstract class BlockElement : Element
    {
        [TypeConverter (typeof(LengthConverter))]
        public int? Width { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? Height { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? MinWidth { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? MinHeight { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? MaxWidth { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? MaxHeight { get; set; }

        [TypeConverter (typeof(ColorConverter))]
        public ConsoleColor? Color { get; set; }

        [TypeConverter (typeof(ColorConverter))]
        public ConsoleColor? BgColor { get; set; }

        public HorizontalAlignment Align { get; set; }

        public VerticalAlignment VAlign { get; set; }

        protected BlockElement ()
        {
            Align = HorizontalAlignment.Stretch;
            VAlign = VerticalAlignment.Stretch;
        }
    }
}