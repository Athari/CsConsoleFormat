﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class Canvas : ContainerElement
    {
        public static readonly AttachedProperty<int?> LeftProperty = RegisterAttached(() => LeftProperty);
        public static readonly AttachedProperty<int?> TopProperty = RegisterAttached(() => TopProperty);
        public static readonly AttachedProperty<int?> RightProperty = RegisterAttached(() => RightProperty);
        public static readonly AttachedProperty<int?> BottomProperty = RegisterAttached(() => BottomProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetLeft ([NotNull] BlockElement @this) => @this.GetValueSafe(LeftProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetTop ([NotNull] BlockElement @this) => @this.GetValueSafe(TopProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetRight ([NotNull] BlockElement @this) => @this.GetValueSafe(RightProperty);

        [TypeConverter (typeof(LengthConverter))]
        public static int? GetBottom ([NotNull] BlockElement @this) => @this.GetValueSafe(BottomProperty);

        public static void SetLeft ([NotNull] BlockElement @this, int? value) => @this.SetValueSafe(LeftProperty, value);
        public static void SetTop ([NotNull] BlockElement @this, int? value) => @this.SetValueSafe(TopProperty, value);
        public static void SetRight ([NotNull] BlockElement @this, int? value) => @this.SetValueSafe(RightProperty, value);
        public static void SetBottom ([NotNull] BlockElement @this, int? value) => @this.SetValueSafe(BottomProperty, value);

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size MeasureOverride (Size availableSize)
        {
            var childAvailableSize = new Size(Size.Infinity, Size.Infinity);
            foreach (BlockElement child in VisualChildren)
                child.Measure(childAvailableSize);
            return new Size(0, 0);
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        protected override Size ArrangeOverride (Size finalSize)
        {
            foreach (BlockElement child in VisualChildren) {
                int x = 0, y = 0;

                int? left = GetLeft(child);
                if (left != null)
                    x = left.Value;
                else {
                    int? right = GetRight(child);
                    if (right != null)
                        x = finalSize.Width - child.DesiredSize.Width - right.Value;
                }

                int? top = GetTop(child);
                if (top != null)
                    y = top.Value;
                else {
                    int? bottom = GetBottom(child);
                    if (bottom != null)
                        y = finalSize.Height - child.DesiredSize.Height - bottom.Value;
                }

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
            }
            return finalSize;
        }

        private static AttachedProperty<T> RegisterAttached<T> (Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T)) =>
            AttachedProperty.Register<Canvas, T>(nameExpression, defaultValue);
    }
}