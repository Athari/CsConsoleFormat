namespace Alba.CsConsoleFormat
{
    internal class LayoutInfo
    {
        public Vector ActualOffset { get; set; }
        public Size DesiredSize { get; set; }
        public Rect LayoutClip { get; set; }
        public Size RenderSize { get; set; }
        public Rect RenderSlotRect { get; set; }
        public Size UnclippedDesiredSize { get; set; }

        public LayoutInfo Clone () => (LayoutInfo)MemberwiseClone();
    }
}