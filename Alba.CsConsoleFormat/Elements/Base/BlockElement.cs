using System;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    public abstract class BlockElement : Element
    {
        private LayoutInfo layoutInfo = new LayoutInfo();

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

        public TextWrapping TextWrapping { get; set; }

        public Thickness Margin { get; set; }

        protected BlockElement ()
        {
            MinWidth = 0;
            MinHeight = 0;
            MaxWidth = Size.Infinity;
            MaxHeight = Size.Infinity;
            Align = HorizontalAlignment.Stretch;
            VAlign = VerticalAlignment.Stretch;
            TextAlign = HorizontalAlignment.Left;
            TextWrapping = TextWrapping.WordWrap;
        }

        /// <summary>Render area width.</summary><seealso cref="RenderSize"/>
        public int ActualWidth
        {
            get { return RenderSize.Width; }
        }

        /// <summary>Render area height.</summary><seealso cref="RenderSize"/>
        public int ActualHeight
        {
            get { return RenderSize.Height; }
        }

        /// <summary>Element position (relative to visual parent).</summary>
        public Vector ActualOffset
        {
            get { return layoutInfo.ActualOffset; }
            private set { layoutInfo.ActualOffset = value; }
        }

        /// <summary>Element size returned by <see cref="Measure"/>, constrained by max element size and available size with margins.</summary>
        public Size DesiredSize
        {
            get { return layoutInfo.DesiredSize; }
            private set { layoutInfo.DesiredSize = value; }
        }

        /// <summary>Render area constraint (relative to visual parent).</summary>
        internal Rect LayoutClip
        {
            get { return layoutInfo.LayoutClip; }
            private set { layoutInfo.LayoutClip = value; }
        }

        /// <summary>Render area size.</summary><seealso cref="ActualWidth"/><seealso cref="ActualHeight"/>
        public Size RenderSize
        {
            get { return layoutInfo.RenderSize; }
            private set { layoutInfo.RenderSize = value; }
        }

        /// <summary>Area occupied by element, including margins (relative to visual parent).</summary>
        public Rect RenderSlotRect
        {
            get { return layoutInfo.RenderSlotRect; }
            private set { layoutInfo.RenderSlotRect = value; }
        }

        /// <summary>Element size returned by <see cref="Measure"/>, expanded by min element size.</summary><seealso cref="DesiredSize"/>
        private Size UnclippedDesiredSize
        {
            get { return layoutInfo.UnclippedDesiredSize; }
            set { layoutInfo.UnclippedDesiredSize = value; }
        }

        public void Measure (Size availableSize)
        {
            if (Visibility == Visibility.Collapsed) {
                UnclippedDesiredSize = Size.Empty;
                DesiredSize = Size.Empty;
                return;
            }

            // Apply margin. availableSize is what parent want us to be.
            Size constrainedAvailableSize = new Size(availableSize.Width - Margin.Width, availableSize.Height - Margin.Height, false);

            // Apply min/max/currentvalue constraints.
            MinMaxSize mm = new MinMaxSize(MinHeight, MaxHeight, MinWidth, MaxWidth, Width, Height);
            constrainedAvailableSize = Size.MinMax(constrainedAvailableSize, mm.MinSize, mm.MaxSize);

            Size desiredSize = MeasureOverride(constrainedAvailableSize);
            if (desiredSize.IsInfinite)
                throw new InvalidOperationException("MeasureOverride must return finite size.");

            // Maximize desiredSize with user provided min size.
            desiredSize = Size.Max(desiredSize, mm.MinSize);

            // Here is the "true minimum" desired size - the one that is for sure enough for the control to render its content.
            // UnclippedDesiredSize is needed in ArrangeCore, because due to the layout protocol, arrange should be called
            // with constraints greater or equal to child's desired size returned from MeasureOverride.
            UnclippedDesiredSize = desiredSize;

            // User-specified max size starts to "clip" the control here.
            // Starting from this point desiredSize could be smaller than actually needed to render the whole control.
            desiredSize = Size.Min(desiredSize, mm.MaxSize);

            // Because of negative margins, clipped desired size may be negative.
            // In overconstrained scenario, parent wins and measured size of the child, including any sizes set or computed,
            // cannot be larger than available size. We will clip it later.
            DesiredSize = Size.Min(desiredSize + Margin, availableSize);
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

            // Start to compute arrange size for the child. It starts from layout slot or deisred size if layout slot is smaller than desired,
            // and then we reduce it by margins, apply Width/Height etc, to arrive at the size that child will get in its ArrangeOverride.
            // Next, compare against unclipped, transformed size.
            Size arrangeSize = Size.Max(finalRect.Size - Margin, UnclippedDesiredSize);

            // Alignment == Stretch --> arrange at the slot size minus margins; Alignment != Stretch --> arrange at the UnclippedDesiredSize.
            if (Align != HorizontalAlignment.Stretch)
                arrangeSize.Width = UnclippedDesiredSize.Width;
            if (VAlign != VerticalAlignment.Stretch)
                arrangeSize.Height = UnclippedDesiredSize.Height;

            // Here we use un-clipped RenderSize because element does not know that it is clipped by layout system and it should have
            // as much space to render as it returned from its own ArrangeOverride.
            RenderSize = ArrangeOverride(arrangeSize);
            ActualOffset = CalculateAlignmentOffset() + new Vector(finalRect.X + Margin.Left, finalRect.Y + Margin.Top);
            LayoutClip = CalculateLayoutClip();
        }

        protected virtual Size ArrangeOverride (Size finalSize)
        {
            BlockElement child = VisualChild;
            if (child != null)
                child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private Rect CalculateLayoutClip ()
        {
            return new Rect(-CalculateAlignmentOffset(), CalculateClientSize());
        }

        private Vector CalculateAlignmentOffset ()
        {
            // Clipped RenderSize differs from RenderSize only what MaxWidth/Height explicitly clip the otherwise good arrangement. 
            // For ex, DS<clientSize but DS>MaxWidth - in this case we should initiate clip at MaxWidth and only show Top-Left portion
            // of the element limited by Max properties. It is Top-left because in case when we are clipped by container we also degrade
            // to Top-Left, so we are consistent.
            MinMaxSize mm = new MinMaxSize(MinHeight, MaxHeight, MinWidth, MaxWidth, Width, Height);
            return CalculateAlignmentOffsetCore(CalculateClientSize(), Size.Min(RenderSize, mm.MaxSize));
        }

        private Size CalculateClientSize ()
        {
            return new Size(RenderSlotRect.Width - Margin.Width, RenderSlotRect.Height - Margin.Height, false);
        }

        private Vector CalculateAlignmentOffsetCore (Size clientSize, Size renderSize)
        {
            Vector offset = new Vector();

            HorizontalAlignment halign = Align;
            VerticalAlignment valign = VAlign;

            // This is to degenerate Stretch to Top-Left in case when clipping is about to occur.
            /*if (halign == HorizontalAlignment.Stretch && renderSize.Width > clientSize.Width)
                halign = HorizontalAlignment.Left;
            if (valign == VerticalAlignment.Stretch && renderSize.Height > clientSize.Height)
                valign = VerticalAlignment.Top;*/

            if (halign == HorizontalAlignment.Center || halign == HorizontalAlignment.Stretch)
                offset.X = (clientSize.Width - renderSize.Width) / 2;
            else if (halign == HorizontalAlignment.Right)
                offset.X = clientSize.Width - renderSize.Width;
            else
                offset.X = 0;

            if (valign == VerticalAlignment.Center || valign == VerticalAlignment.Stretch)
                offset.Y = (clientSize.Height - renderSize.Height) / 2;
            else if (valign == VerticalAlignment.Bottom)
                offset.Y = clientSize.Height - renderSize.Height;
            else
                offset.Y = 0;

            return offset;
        }

        public virtual void Render (ConsoleRenderBuffer buffer)
        {
            if (BgColor != null)
                buffer.FillBackgroundRectangle(0, 0, RenderSize.Width, RenderSize.Height, BgColor.Value);
        }

        protected override void CloneOverride (Element sourceElement)
        {
            var source = (BlockElement)sourceElement;
            base.CloneOverride(source);
            layoutInfo = source.layoutInfo.Clone();
        }

        private static int MinMax (int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        private struct MinMaxSize
        {
            public MinMaxSize (int minHeight, int maxHeight, int minWidth, int maxWidth, int? width, int? height) : this()
            {
                MaxHeight = MinMax(height ?? Size.Infinity, minHeight, maxHeight);
                MinHeight = MinMax(height ?? 0, minHeight, MaxHeight);
                MaxWidth = MinMax(width ?? Size.Infinity, minWidth, maxWidth);
                MinWidth = MinMax(width ?? 0, minWidth, MaxWidth);
            }

            private int MinWidth { get; set; }
            private int MaxWidth { get; set; }
            private int MinHeight { get; set; }
            private int MaxHeight { get; set; }

            public Size MaxSize
            {
                get { return new Size(MaxWidth, MaxHeight); }
            }

            public Size MinSize
            {
                get { return new Size(MinWidth, MinHeight); }
            }
        }
    }
}