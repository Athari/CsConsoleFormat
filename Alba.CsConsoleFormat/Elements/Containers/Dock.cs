﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class Dock : ContainerElement
    {
        public static readonly AttachedProperty<DockTo> ToProperty = RegisterAttached(() => ToProperty);

        public bool LastChildFill { get; set; } = true;

        [Pure]
        public static DockTo GetTo([NotNull] BlockElement @this) => @this.GetValueSafe(ToProperty);

        public static void SetTo([NotNull] BlockElement @this, DockTo value) => @this.SetValueSafe(ToProperty, value);

        public Dock()
        { }

        public Dock(params object[] children) : base(children)
        { }

        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size MeasureOverride(Size availableSize)
        {
            Size parentSize = new Size(0, 0);
            Size accumulated = new Size(0, 0);

            foreach (BlockElement child in VisualChildren) {
                child.Measure(availableSize - accumulated);
                Size childSize = child.DesiredSize;
                switch (GetTo(child)) {
                    case DockTo.Left:
                    case DockTo.Right:
                        parentSize.Height = Math.Max(parentSize.Height, accumulated.Height + childSize.Height);
                        accumulated.Width += childSize.Width;
                        break;
                    case DockTo.Top:
                    case DockTo.Bottom:
                        parentSize.Width = Math.Max(parentSize.Width, accumulated.Width + childSize.Width);
                        accumulated.Height += childSize.Height;
                        break;
                }
            }

            return Size.Max(parentSize, accumulated);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int dockedChildrenCount = VisualChildren.Count - (LastChildFill ? 1 : 0);
            Thickness accumulated = 0;

            for (int i = 0; i < VisualChildren.Count; ++i) {
                var child = (BlockElement)VisualChildren[i];
                Size childSize = child.DesiredSize;
                Rect childRect = new Rect(
                    new Point(accumulated.Left, accumulated.Top),
                    new Size(finalSize.Width - accumulated.Width, finalSize.Height - accumulated.Height, false));
                if (i < dockedChildrenCount) {
                    switch (GetTo(child)) {
                        case DockTo.Left:
                            accumulated.Left += childSize.Width;
                            childRect.Width = childSize.Width;
                            break;
                        case DockTo.Right:
                            accumulated.Right += childSize.Width;
                            childRect.X = Math.Max(0, finalSize.Width - accumulated.Right);
                            childRect.Width = childSize.Width;
                            break;
                        case DockTo.Top:
                            accumulated.Top += childSize.Height;
                            childRect.Height = childSize.Height;
                            break;
                        case DockTo.Bottom:
                            accumulated.Bottom += childSize.Height;
                            childRect.Y = Math.Max(0, finalSize.Height - accumulated.Bottom);
                            childRect.Height = childSize.Height;
                            break;
                    }
                }
                child.Arrange(childRect);
            }
            return finalSize;
        }

        [MustUseReturnValue]
        private static AttachedProperty<T> RegisterAttached<T>([NotNull] Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default) =>
            AttachedProperty.Register<Dock, T>(nameExpression, defaultValue);
    }
}