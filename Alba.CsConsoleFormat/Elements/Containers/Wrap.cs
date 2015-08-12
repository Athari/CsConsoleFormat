using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public class Wrap : ContainerElement
    {
        private int? _itemWidth;
        private int? _itemHeight;

        public Orientation Orientation { get; set; }

        public Wrap ()
        {
            Orientation = Orientation.Horizontal;
        }

        [TypeConverter (typeof(LengthConverter))]
        public int? ItemWidth
        {
            get { return _itemWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "ItemWidth cannot be negative.");
                _itemWidth = value;
            }
        }

        [TypeConverter (typeof(LengthConverter))]
        public int? ItemHeight
        {
            get { return _itemHeight; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "ItemHeight cannot be negative.");
                _itemHeight = value;
            }
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size MeasureOverride (Size availableSize)
        {
            UVSize curLineSize = new UVSize(Orientation);
            UVSize panelSize = new UVSize(Orientation);
            UVSize availableSizeUV = new UVSize(Orientation, availableSize);
            Size childAvailableSize = OverrideSize(availableSize);

            foreach (BlockElement child in VisualChildren) {
                child.Measure(childAvailableSize);

                UVSize childSize = new UVSize(Orientation, OverrideSize(child.DesiredSize));
                // Need to switch to another line.
                if (curLineSize.U + childSize.U > availableSizeUV.U) {
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = childSize;
                    // If the element is wider than the constraint, give it a separate line.
                    if (childSize.U > availableSizeUV.U) {
                        panelSize.U = Math.Max(childSize.U, panelSize.U);
                        panelSize.V += childSize.V;
                        curLineSize = new UVSize(Orientation);
                    }
                }
                // Continue to accumulate a line.
                else {
                    curLineSize.U += childSize.U;
                    curLineSize.V = Math.Max(childSize.V, curLineSize.V);
                }
            }
            // Add size of the last line.
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            return new Size(panelSize.Width, panelSize.Height);
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            int firstInLine = 0;
            int accumulatedV = 0;
            int? itemU = GetU(ItemWidth, ItemHeight);
            UVSize curLineSize = new UVSize(Orientation);
            UVSize finalSizeUV = new UVSize(Orientation, finalSize);

            for (int i = 0; i < VisualChildren.Count; i++) {
                var child = (BlockElement)VisualChildren[i];

                UVSize childSize = new UVSize(Orientation, OverrideSize(child.DesiredSize));
                // Need to switch to another line.
                if (curLineSize.U + childSize.U > finalSizeUV.U) {
                    ArrangeLine(accumulatedV, curLineSize.V, firstInLine, i, itemU);
                    accumulatedV += curLineSize.V;
                    curLineSize = childSize;
                    // If the element is wider than the constraint, give it a separate line.
                    if (childSize.U > finalSizeUV.U) {
                        // Switch to next line which only contains one element.
                        ArrangeLine(accumulatedV, childSize.V, i, ++i, itemU);
                        accumulatedV += childSize.V;
                        curLineSize = new UVSize(Orientation);
                    }
                    firstInLine = i;
                }
                // Continue to accumulate a line.
                else {
                    curLineSize.U += childSize.U;
                    curLineSize.V = Math.Max(childSize.V, curLineSize.V);
                }
            }
            // Arrange the last line, if any.
            if (firstInLine < VisualChildren.Count)
                ArrangeLine(accumulatedV, curLineSize.V, firstInLine, VisualChildren.Count, itemU);

            return finalSize;
        }

        private void ArrangeLine (int v, int lineV, int start, int end, int? itemU)
        {
            for (int i = start, u = 0, layoutSlotU; i < end; i++, u += layoutSlotU) {
                var child = (BlockElement)VisualChildren[i];
                UVSize childSize = new UVSize(Orientation, child.DesiredSize);
                layoutSlotU = itemU ?? childSize.U;
                child.Arrange(new Rect(
                    GetU(u, v),
                    GetV(u, v),
                    GetU(layoutSlotU, lineV),
                    GetV(layoutSlotU, lineV)));
            }
        }

        private T GetU<T> (T u, T v)
        {
            return Orientation == Orientation.Horizontal ? u : v;
        }

        private T GetV<T> (T u, T v)
        {
            return Orientation == Orientation.Horizontal ? v : u;
        }

        private Size OverrideSize (Size size)
        {
            return new Size(ItemWidth ?? size.Width, ItemHeight ?? size.Height);
        }

        private struct UVSize
        {
            private readonly Orientation _orientation;

            public int U;
            public int V;

            public UVSize (Orientation orientation, int width = 0, int height = 0)
            {
                _orientation = orientation;
                U = _orientation == Orientation.Horizontal ? width : height;
                V = _orientation == Orientation.Horizontal ? height : width;
            }

            public UVSize (Orientation orientation, Size size) : this(orientation, size.Width, size.Height)
            {}

            public int Width
            {
                get { return _orientation == Orientation.Horizontal ? U : V; }
            }

            public int Height
            {
                get { return _orientation == Orientation.Horizontal ? V : U; }
            }
        }
    }
}