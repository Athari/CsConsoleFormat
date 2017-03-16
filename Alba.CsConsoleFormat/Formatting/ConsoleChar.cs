using System;
using System.Globalization;
using static System.FormattableString;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Alba.CsConsoleFormat
{
    public struct ConsoleChar : IEquatable<ConsoleChar>
    {
        private char _char;
        private byte _colors;
        private byte _state;

        public char Char
        {
            get => _char;
            set => _char = value;
        }

        public bool HasChar => Char != '\0';

        public char PrintableChar
        {
            get
            {
                if (_char < ' ')
                    return ' ';
                if (_char == Chars.NoBreakHyphen || _char == Chars.SoftHyphen)
                    return '-';
                if (_char == Chars.NoBreakSpace || _char == Chars.ZeroWidthSpace)
                    return ' ';
                return _char;
            }
        }

        public ConsoleColor ForegroundColor
        {
            get => (ConsoleColor)GetBits(_colors, 0, 0xF);
            set => SetBits(ref _colors, (byte)value, 0, 0xF);
        }

        public ConsoleColor BackgroundColor
        {
            get => (ConsoleColor)GetBits(_colors, 4, 0xF);
            set => SetBits(ref _colors, (byte)value, 4, 0xF);
        }

        public LineChar LineChar
        {
            get => (LineChar)GetBits(_state, 0, 0xF);
            set => SetBits(ref _state, (byte)value, 0, 0xF);
        }

        public LineWidth LineWidthHorizontal
        {
            get => (LineWidth)GetBits(_state, 0, 0x3);
            set => SetBits(ref _state, (byte)value, 0, 0x3);
        }

        public LineWidth LineWidthVertical
        {
            get => (LineWidth)GetBits(_state, 2, 0x3);
            set => SetBits(ref _state, (byte)value, 2, 0x3);
        }

        private static byte GetBits(byte data, int offset, byte mask)
        {
            return (byte)((data >> offset) & mask);
        }

        private static void SetBits(ref byte data, byte value, int offset, byte mask)
        {
            data = (byte)((data & ~(mask << offset)) | (value & mask) << offset);
        }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "{0}{1} ({2} @ {3})",
                _char >= ' ' ? _char.ToString() : "#" + (int)_char,
                (LineChar != LineChar.None ? Invariant($" ({LineWidthHorizontal} x {LineWidthVertical})") : ""),
                ForegroundColor,
                BackgroundColor);

        public bool Equals(ConsoleChar other) => _char == other._char && _colors == other._colors && _state == other._state;
        public override bool Equals(object obj) => obj is ConsoleChar && Equals((ConsoleChar)obj);
        public override int GetHashCode() => _char.GetHashCode() ^ _colors.GetHashCode() ^ _state.GetHashCode();

        public static bool operator ==(ConsoleChar left, ConsoleChar right) => left.Equals(right);
        public static bool operator !=(ConsoleChar left, ConsoleChar right) => !left.Equals(right);
    }
}