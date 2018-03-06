using System;

namespace Alba.CsConsoleFormat
{
    [Flags]
    public enum DrawLineFlags
    {
        None = 0,

        CapStartCenter = 1 << 0,
        CapStartFull = CapStartCenter | (1 << 1),
        CapEndCenter = 1 << 2,
        CapEndFull = CapEndCenter | (1 << 3),

        CapCenter = CapStartCenter | CapEndCenter,
        CapFull = CapStartFull | CapEndFull,
    }

    internal static class DrawLineFlagsExts
    {
        public static bool HasCapStartCenter(this DrawLineFlags @this) => (@this & DrawLineFlags.CapStartCenter) == DrawLineFlags.CapStartCenter;
        public static bool HasCapStartFull(this DrawLineFlags @this) => (@this & DrawLineFlags.CapStartFull) == DrawLineFlags.CapStartFull;
        public static bool HasCapEndCenter(this DrawLineFlags @this) => (@this & DrawLineFlags.CapEndCenter) == DrawLineFlags.CapEndCenter;
        public static bool HasCapEndFull(this DrawLineFlags @this) => (@this & DrawLineFlags.CapEndFull) == DrawLineFlags.CapEndFull;
    }
}