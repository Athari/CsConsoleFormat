using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat.Framework.Sys
{
    internal static class Bits
    {
        [Pure]
        public static byte Get(byte data, int offset, int size)
        {
            int mask = GetMask(size);
            return (byte)((data >> offset) & mask);
        }

        public static void Set(ref byte data, byte value, int offset, int size)
        {
            int mask = GetMask(size);
            data = (byte)((data & ~(mask << offset)) | (value & mask) << offset);
        }

        private static int GetMask(int size) => (1 << size) - 1;
    }
}