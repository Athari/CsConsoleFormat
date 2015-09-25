using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public enum LineWidth
    {
        None = 0,
        Single = 1,
        Wide = 2,
    }

    internal static class LineWidthExts
    {
        public static int ToCharWidth (this LineWidth @this) => @this == LineWidth.None ? 0 : 1;

        public static LineWidth Max (LineWidth left, LineWidth right) => (LineWidth)Math.Max((int)left, (int)right);

        public static LineWidth Max ([InstantHandle] IEnumerable<LineWidth> items) => items.Aggregate(LineWidth.None, Max);
    }
}