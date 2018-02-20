using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat
{
    internal sealed class LayoutInfo
    {
        public Vector ActualOffset { get; set; }
        public Size DesiredSize { get; set; }
        public Rect LayoutClip { get; set; }
        public Size RenderSize { get; set; }
        public Rect RenderSlotRect { get; set; }
        public Size UnclippedDesiredSize { get; set; }

        [Pure]
        public LayoutInfo Clone() => (LayoutInfo)MemberwiseClone();
    }
}