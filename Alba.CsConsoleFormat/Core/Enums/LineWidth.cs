using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public enum LineWidth
    {
        None,
        Single,
        Heavy,
        Double,
    }

    internal static class LineWidthExts
    {
        public static int ToCharWidth(this LineWidth @this) => @this == LineWidth.None ? 0 : 1;

        [Pure]
        public static LineWidth Max(LineWidth left, LineWidth right) => (LineWidth)Math.Max((int)left, (int)right);

        [Pure]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "API consistency.")]
        public static LineWidth Max([NotNull, InstantHandle] IEnumerable<LineWidth> items) => items.Aggregate(LineWidth.None, Max);
    }
}