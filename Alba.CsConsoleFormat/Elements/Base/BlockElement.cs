using System;
using System.ComponentModel;
using System.Linq;

namespace Alba.CsConsoleFormat
{
    public abstract class BlockElement : Element
    {
        private readonly LayoutInfo layoutInfo = new LayoutInfo();

        [TypeConverter (typeof(LengthConverter))]
        public int? Width { get; set; }

        [TypeConverter (typeof(LengthConverter))]
        public int? Height { get; set; }

        public int MinWidth { get; set; }

        public int MinHeight { get; set; }

        public int MaxWidth { get; set; }

        public int MaxHeight { get; set; }

        public HorizontalAlignment Align { get; set; }

        public VerticalAlignment VAlign { get; set; }

        public HorizontalAlignment TextAlign { get; set; }

        public Thickness Margin { get; set; }

        protected BlockElement ()
        {
            MinWidth = 0;
            MinHeight = 0;
            MaxWidth = int.MaxValue;
            MaxHeight = int.MaxValue;
            Align = HorizontalAlignment.Stretch;
            VAlign = VerticalAlignment.Stretch;
        }

        public Vector ActualOffset
        {
            get { return layoutInfo.actualOffset; }
            private set { layoutInfo.actualOffset = value; }
        }

        public Size DesiredSize
        {
            get { return layoutInfo.desiredSize; }
            set { layoutInfo.desiredSize = value; }
        }

        private Size UnclippedDesiredSize
        {
            get { return layoutInfo.unclippedDesiredSize; }
            set { layoutInfo.unclippedDesiredSize = value; }
        }

        public Size RenderSize
        {
            get { return layoutInfo.renderSize; }
            private set { layoutInfo.renderSize = value; }
        }

        public Rect RenderSlotRect
        {
            get { return layoutInfo.renderSlotRect; }
            private set { layoutInfo.renderSlotRect = value; }
        }

        internal Rect LayoutClip
        {
            get { return layoutInfo.layoutClip; }
            private set { layoutInfo.layoutClip = value; }
        }

        public void Measure (Size availableSize)
        {
            layoutInfo.measureArgument = availableSize;

            if (Visibility == Visibility.Collapsed) {
                UnclippedDesiredSize = Size.Empty;
                DesiredSize = Size.Empty;
                return;
            }

            // Apply margin.
            int marginWidth = Margin.Width, marginHeight = Margin.Height;

            // Parent size is what parent want us to be.
            Size constrainedAvailableSize = new Size(
                Math.Max(availableSize.Width - marginWidth, 0),
                Math.Max(availableSize.Height - marginHeight, 0));

            // Apply min/max/currentvalue constraints.
            MinMaxSize mm = new MinMaxSize(MinHeight, MaxHeight, MinWidth, MaxWidth, Width, Height);
            constrainedAvailableSize.Width = MinMax(constrainedAvailableSize.Width, mm.MinWidth, mm.MaxWidth);
            constrainedAvailableSize.Height = MinMax(constrainedAvailableSize.Height, mm.MinHeight, mm.MaxHeight);

            Size desiredSize = MeasureOverride(constrainedAvailableSize);
            if (!desiredSize.IsFinite)
                throw new InvalidOperationException("MeasureOverride must return finite size.");

            // Maximize desiredSize with user provided min size.
            desiredSize = new Size(Math.Max(desiredSize.Width, mm.MinWidth), Math.Max(desiredSize.Height, mm.MinHeight));

            // Here is the "true minimum" desired size - the one that is for sure enough for the control to render its content.
            Size unclippedDesiredSize = desiredSize;

            // User-specified max size starts to "clip" the control here.
            // Starting from this point desiredSize could be smaller then actually needed to render the whole control.
            if (desiredSize.Width > mm.MaxWidth)
                desiredSize.Width = mm.MaxWidth;
            if (desiredSize.Height > mm.MaxHeight)
                desiredSize.Height = mm.MaxHeight;

            // Because of negative margins, clipped desired size may be negative.
            // Need to keep it as ints for that reason and maximize with 0 at the very last point - before returning desired size to the parent.
            int clippedDesiredWidth = desiredSize.Width + marginWidth, clippedDesiredHeight = desiredSize.Height + marginHeight;

            // In overconstrained scenario, parent wins and measured size of the child, including any sizes set or computed,
            // can not be larger then available size. We will clip the guy later.
            if (clippedDesiredWidth > availableSize.Width)
                clippedDesiredWidth = availableSize.Width;
            if (clippedDesiredHeight > availableSize.Height)
                clippedDesiredHeight = availableSize.Height;

            // Note: unclippedDesiredSize is needed in ArrangeCore, because due to the layout protocol, arrange should be called
            // with constraints greater or equal to child's desired size returned from MeasureOverride.
            UnclippedDesiredSize = unclippedDesiredSize;
            DesiredSize = new Size(Math.Max(0, clippedDesiredWidth), Math.Max(0, clippedDesiredHeight));
        }

        protected virtual Size MeasureOverride (Size availableSize)
        {
            BlockElement child = VisualChild;
            if (child == null)
                return new Size(0, 0);
            child.Measure(availableSize);
            return child.DesiredSize;
        }

        public void Arrange (Rect finalRect)
        {
            if (Visibility == Visibility.Collapsed) {
                RenderSlotRect = Rect.Empty;
                RenderSize = Size.Empty;
                LayoutClip = CalculateLayoutClip();
                return;
            }

            RenderSlotRect = finalRect;

            // Start to compute arrange size for the child. It starts from layout slot or deisred size if layout slot is smaller then desired,
            // and then we reduce it by margins, apply Width/Height etc, to arrive at the size that child will get in its ArrangeOverride.
            int marginWidth = Margin.Width, marginHeight = Margin.Height;
            Size arrangeSize = finalRect.Size;
            arrangeSize.Width = Math.Max(0, arrangeSize.Width - marginWidth);
            arrangeSize.Height = Math.Max(0, arrangeSize.Height - marginHeight);

            // Next, compare against unclipped, transformed size.
            Size unclippedDesiredSize = UnclippedDesiredSize;

            if (arrangeSize.Width < unclippedDesiredSize.Width)
                arrangeSize.Width = unclippedDesiredSize.Width;
            if (arrangeSize.Height < unclippedDesiredSize.Height)
                arrangeSize.Height = unclippedDesiredSize.Height;

            // Alignment==Stretch --> arrange at the slot size minus margins
            // Alignment!=Stretch --> arrange at the unclippedDesiredSize 
            if (Align != HorizontalAlignment.Stretch)
                arrangeSize.Width = unclippedDesiredSize.Width;
            if (VAlign != VerticalAlignment.Stretch)
                arrangeSize.Height = unclippedDesiredSize.Height;

            // Here we use un-clipped InkSize because element does not know that it is clipped by layout system and it should have
            // as much space to render as it returned from its own ArrangeOverride.
            RenderSize = ArrangeOverride(arrangeSize);

            Vector offset = CalculateAlignmentOffset();
            offset.X += finalRect.X + Margin.Left;
            offset.Y += finalRect.Y + Margin.Top;
            if (!ActualOffset.Equals(offset))
                ActualOffset = offset;

            LayoutClip = CalculateLayoutClip();
        }

        protected virtual Size ArrangeOverride (Size finalSize)
        {
            BlockElement child = VisualChild;
            if (child != null)
                child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public virtual void Render (ConsoleRenderBuffer buffer)
        {}

        private Rect CalculateLayoutClip ()
        {
            Vector offset = CalculateAlignmentOffset();
            Size clientSize = GetClientSize();
            return new Rect(-offset.X, -offset.Y, clientSize.Width, clientSize.Height);
        }

        private Vector CalculateAlignmentOffset ()
        {
            // clippedInkSize differs from InkSize only what MaxWidth/Height explicitly clip the
            // otherwise good arrangement. For ex, DS<clientSize but DS>MaxWidth - in this
            // case we should initiate clip at MaxWidth and only show Top-Left portion
            // of the element limited by Max properties. It is Top-left because in case when we
            // are clipped by container we also degrade to Top-Left, so we are consistent.
            MinMaxSize mm = new MinMaxSize(MinHeight, MaxHeight, MinWidth, MaxWidth, Width, Height);
            Size renderSize = RenderSize;
            Size clippedInkSize = new Size(Math.Min(renderSize.Width, mm.MaxWidth), Math.Min(renderSize.Height, mm.MaxHeight));
            Size clientSize = GetClientSize();
            return CalculateAlignmentOffsetCore(clientSize, clippedInkSize);
        }

        private Size GetClientSize ()
        {
            int marginWidth = Margin.Width, marginHeight = Margin.Height;
            Rect renderSlotRect = RenderSlotRect;
            return new Size(
                Math.Max(0, renderSlotRect.Width - marginWidth),
                Math.Max(0, renderSlotRect.Height - marginHeight));
        }

        private Vector CalculateAlignmentOffsetCore (Size clientSize, Size inkSize)
        {
            Vector offset = new Vector();

            HorizontalAlignment halign = Align;
            VerticalAlignment valign = VAlign;

            // This is to degenerate Stretch to Top-Left in case when clipping is about to occur.
            if (halign == HorizontalAlignment.Stretch && inkSize.Width > clientSize.Width)
                halign = HorizontalAlignment.Left;
            if (valign == VerticalAlignment.Stretch && inkSize.Height > clientSize.Height)
                valign = VerticalAlignment.Top;

            if (halign == HorizontalAlignment.Center || halign == HorizontalAlignment.Stretch)
                offset.X = (clientSize.Width - inkSize.Width) / 2;
            else if (halign == HorizontalAlignment.Right)
                offset.X = clientSize.Width - inkSize.Width;
            else
                offset.X = 0;

            if (valign == VerticalAlignment.Center || valign == VerticalAlignment.Stretch)
                offset.Y = (clientSize.Height - inkSize.Height) / 2;
            else if (valign == VerticalAlignment.Bottom)
                offset.Y = clientSize.Height - inkSize.Height;
            else
                offset.Y = 0;

            return offset;
        }

        private static int MinMax (int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        private struct MinMaxSize
        {
            public MinMaxSize (int minHeight, int maxHeight, int minWidth, int maxWidth, int? width, int? height) : this()
            {
                MaxHeight = MinMax(height ?? int.MaxValue, minHeight, maxHeight);
                MinHeight = MinMax(height ?? 0, minHeight, MaxHeight);
                MaxWidth = MinMax(width ?? int.MaxValue, minWidth, maxWidth);
                MinWidth = MinMax(width ?? 0, minWidth, MaxWidth);
            }

            public int MinWidth { get; private set; }
            public int MaxWidth { get; private set; }
            public int MinHeight { get; private set; }
            public int MaxHeight { get; private set; }
        }
    }
}