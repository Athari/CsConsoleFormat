using System;
using System.Globalization;
using Alba.CsConsoleFormat.Framework.Sys;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Alba.CsConsoleFormat
{
    public struct ConsoleChar : IEquatable<ConsoleChar>
    {
        private byte _colors;

        public char Char { get; set; }
        public LineChar LineChar { get; set; }

        public bool HasChar => Char != '\0';

        public char PrintableChar
        {
            get
            {
                if (Char < ' ')
                    return ' ';
                if (Char == Chars.NoBreakHyphen || Char == Chars.SoftHyphen)
                    return '-';
                if (Char == Chars.NoBreakSpace || Char == Chars.ZeroWidthSpace)
                    return ' ';
                return Char;
            }
        }

        public ConsoleColor ForegroundColor
        {
            get => (ConsoleColor)Bits.Get(_colors, 0, 4);
            set => Bits.Set(ref _colors, (byte)value, 0, 4);
        }

        public ConsoleColor BackgroundColor
        {
            get => (ConsoleColor)Bits.Get(_colors, 4, 4);
            set => Bits.Set(ref _colors, (byte)value, 4, 4);
        }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "{0}{1} ({2} @ {3})",
                Char >= ' ' ? Char.ToString() : "#" + (int)Char,
                LineChar != LineChar.None ? $" {LineChar}" : "",
                ForegroundColor,
                BackgroundColor);

        public bool Equals(ConsoleChar other) => _colors == other._colors && Char == other.Char && LineChar == other.LineChar;
        public override bool Equals(object obj) => obj is ConsoleChar && Equals((ConsoleChar)obj);
        public override int GetHashCode() => _colors.GetHashCode() ^ Char.GetHashCode() ^ LineChar.GetHashCode();

        public static bool operator ==(ConsoleChar left, ConsoleChar right) => left.Equals(right);
        public static bool operator !=(ConsoleChar left, ConsoleChar right) => !left.Equals(right);
    }
}