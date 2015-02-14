namespace Alba.CsConsoleFormat
{
    internal class LayoutInfo
    {
        public Size measureArgument;
        public Size unclippedDesiredSize;
        public Size desiredSize;
        public Rect renderSlotRect;
        public Size renderSize;
        public Rect layoutClip;
        public Vector actualOffset;

        public LayoutInfo Clone ()
        {
            return (LayoutInfo)MemberwiseClone();
        }
    }
}