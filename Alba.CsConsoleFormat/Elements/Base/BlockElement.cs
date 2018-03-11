using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public abstract class BlockElement : Element
    {
        [TypeConverter(typeof(LengthConverter))]
        public int? Width { get; set; }

        [TypeConverter(typeof(LengthConverter))]
        public int? Height { get; set; }

        public int MinWidth { get; set; } = 0;
        public int MinHeight { get; set; } = 0;
        public int MaxWidth { get; set; } = Size.Infinity;
        public int MaxHeight { get; set; } = Size.Infinity;

        public Align Align { get; set; } = Align.Stretch;
        public VerticalAlign VerticalAlign { get; set; } = VerticalAlign.Stretch;
        public TextAlign TextAlign { get; set; } = TextAlign.Left;
        public TextWrap TextWrap { get; set; } = TextWrap.WordWrap;

        public Thickness Margin { get; set; }

        protected BlockElement()
        { }

        protected BlockElement(params object[] children) : base(children)
        { }

        /// <summary>Render area width.</summary><seealso cref="RenderSize"/>
        public int ActualWidth => RenderSize.Width;

        /// <summary>Render area height.</summary><seealso cref="RenderSize"/>
        public int ActualHeight => RenderSize.Height;

        /// <summary>Element position (relative to visual parent).</summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector ActualOffset { get; private set; }

        /// <summary>Element size returned by <see cref="Measure"/>, constrained by max element size and available size with margins.</summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Size DesiredSize { get; private set; }

        /// <summary>Render area constraint (relative to visual parent).</summary>
        internal Rect LayoutClip { get; private set; }

        /// <summary>Render area size.</summary><seealso cref="ActualWidth"/><seealso cref="ActualHeight"/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Size RenderSize { get; private set; }

        /// <summary>Area occupied by element, including margins (relative to visual parent).</summary>
        private Rect RenderSlotRect { get; set; }

        /// <summary>Element size returned by <see cref="Measure"/>, expanded by min element size.</summary><seealso cref="DesiredSize"/>
        private Size UnclippedDesiredSize { get; set; }

        /// <summary>Argument of the last call to Measure.</summary>
        private Size? PreviousAvailableSize { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Measure(Size availableSize)
        {
            if (PreviousAvailableSize == availableSize || Visibility == Visibility.Collapsed)
                return;
            PreviousAvailableSize = availableSize;

            var mm = new MinMaxSize(this);
            Size constrainedAvailableSize = Size.MinMax(availableSize - Margin, mm.MinSize, mm.MaxSize);

            Size desiredSize = MeasureOverride(constrainedAvailableSize);
            if (desiredSize.IsInfinite)
                throw new InvalidOperationException($"{nameof(MeasureOverride)} must return a finite size.");

            UnclippedDesiredSize = desiredSize = Size.Max(desiredSize, mm.MinSize);
            DesiredSize = Size.Min(Size.Min(desiredSize, mm.MaxSize) + Margin, availableSize);
        }

        protected virtual Size MeasureOverride(Size availableSize)
        {
            BlockElement child = VisualChild;
            if (child == null)
                return new Size(0, 0);
            child.Measure(availableSize);
            return child.DesiredSize;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Arrange(Rect finalRect)
        {
            if (finalRect.IsInfinite)
                throw new ArgumentException($"{nameof(finalRect)} must be finite.", nameof(finalRect));

            if (RenderSlotRect == finalRect || Visibility == Visibility.Collapsed)
                return;
            RenderSlotRect = finalRect;

            Size arrangeSize = Size.Max(RenderSlotRect.Size - Margin, UnclippedDesiredSize);
            if (Align != Align.Stretch)
                arrangeSize.Width = UnclippedDesiredSize.Width;
            if (VerticalAlign != VerticalAlign.Stretch)
                arrangeSize.Height = UnclippedDesiredSize.Height;

            var mm = new MinMaxSize(this);
            arrangeSize = Size.Min(arrangeSize, Size.Max(UnclippedDesiredSize, mm.MaxSize));
            RenderSize = ArrangeOverride(arrangeSize);

            Size clippedRenderSize = Size.Min(RenderSize, mm.MaxSize);
            Vector alignOffset = CalculateAlignOffset(RenderSlotRect.Size - Margin, clippedRenderSize);
            ActualOffset = alignOffset + new Vector(RenderSlotRect.Position) + new Vector(Margin.Left, Margin.Top);

            LayoutClip = new Rect(-alignOffset, RenderSlotRect.Size - Margin)
                .Intersect(new Rect(new Size(
                    mm.MaxSize.IsWidthInfinite ? clippedRenderSize.Width : mm.MaxSize.Width,
                    mm.MaxSize.IsHeightInfinite ? clippedRenderSize.Height : mm.MaxSize.Height
                )));
        }

        protected virtual Size ArrangeOverride(Size finalSize)
        {
            VisualChild?.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private Vector CalculateAlignOffset(Size clientSize, Size clippedRenderSize)
        {
            Vector offset = new Vector();

            Align h = Align;
            VerticalAlign v = VerticalAlign;

            if (h == Align.Stretch && clippedRenderSize.Width > clientSize.Width)
                h = Align.Left;
            if (v == VerticalAlign.Stretch && clippedRenderSize.Height > clientSize.Height)
                v = VerticalAlign.Top;

            if (h == Align.Center || h == Align.Stretch)
                offset.X = (clientSize.Width - clippedRenderSize.Width) / 2;
            else if (h == Align.Right)
                offset.X = clientSize.Width - clippedRenderSize.Width;
            else
                offset.X = 0;

            if (v == VerticalAlign.Center || v == VerticalAlign.Stretch)
                offset.Y = (clientSize.Height - clippedRenderSize.Height) / 2;
            else if (v == VerticalAlign.Bottom)
                offset.Y = clientSize.Height - clippedRenderSize.Height;
            else
                offset.Y = 0;

            return offset;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void Render([NotNull] ConsoleBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (Visibility != Visibility.Visible)
                return;
            RenderOverride(buffer);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void RenderOverride([NotNull] ConsoleBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (Background != null)
                buffer.FillBackgroundRectangle(new Rect(RenderSize), Background.Value);
            buffer.FillForegroundRectangle(new Rect(RenderSize), EffectiveColor);
        }

        [Pure]
        internal static int MinMax(int value, int min, int max) => Math.Max(Math.Min(value, max), min);

        private struct MinMaxSize
        {
            public MinMaxSize(BlockElement el) : this(el.MinHeight, el.MaxHeight, el.MinWidth, el.MaxWidth, el.Width, el.Height)
            { }

            private MinMaxSize(int minHeight, int maxHeight, int minWidth, int maxWidth, int? width, int? height)
            {
                MaxHeight = MinMax(height ?? Size.Infinity, minHeight, maxHeight);
                MinHeight = MinMax(height ?? 0, minHeight, MaxHeight);
                MaxWidth = MinMax(width ?? Size.Infinity, minWidth, maxWidth);
                MinWidth = MinMax(width ?? 0, minWidth, MaxWidth);
            }

            private int MinWidth { get; }
            private int MaxWidth { get; }
            private int MinHeight { get; }
            private int MaxHeight { get; }

            public Size MaxSize => new Size(MaxWidth, MaxHeight);
            public Size MinSize => new Size(MinWidth, MinHeight);
        }
    }
}