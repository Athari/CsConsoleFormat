using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.Framework.Sys;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public struct LineChar
    {
        private byte _widths;

        private LineChar(byte widths)
        {
            _widths = widths;
        }

        public LineChar(LineWidth left, LineWidth top, LineWidth right, LineWidth bottom)
            : this((byte)(((byte)left << 0) | ((byte)right << 2) | ((byte)top << 4) | ((byte)bottom << 6)))
        { }

        public LineChar(LineWidth horizontal, LineWidth vertical)
            : this(horizontal, vertical, horizontal, vertical)
        { }

        public LineWidth Left
        {
            get => (LineWidth)Bits.Get(_widths, 0, 2);
            set => Bits.Set(ref _widths, (byte)value, 0, 2);
        }

        public LineWidth Right
        {
            get => (LineWidth)Bits.Get(_widths, 2, 2);
            set => Bits.Set(ref _widths, (byte)value, 2, 2);
        }

        public LineWidth Top
        {
            get => (LineWidth)Bits.Get(_widths, 4, 2);
            set => Bits.Set(ref _widths, (byte)value, 4, 2);
        }

        public LineWidth Bottom
        {
            get => (LineWidth)Bits.Get(_widths, 6, 2);
            set => Bits.Set(ref _widths, (byte)value, 6, 2);
        }

        public static readonly LineChar None = new LineChar();

        public bool IsEmpty => _widths == 0;

        public void SetHorizontal(LineWidth horizontal)
        {
            byte value = (byte)horizontal;
            Bits.Set(ref _widths, (byte)((value << 2) | value), 0, 4);
        }

        public void SetVertical(LineWidth vertical)
        {
            byte value = (byte)vertical;
            Bits.Set(ref _widths, (byte)((value << 2) | value), 4, 4);
        }

        public bool Equals(LineChar other) => _widths == other._widths;
        public override bool Equals(object obj) => obj is LineChar && Equals((LineChar)obj);
        public override int GetHashCode() => _widths.GetHashCode();

        public static bool operator ==(LineChar left, LineChar right) => left.Equals(right);
        public static bool operator !=(LineChar left, LineChar right) => !left.Equals(right);

        public override string ToString() => $"({Left}-{Right})x({Top}-{Bottom})";
    }
}