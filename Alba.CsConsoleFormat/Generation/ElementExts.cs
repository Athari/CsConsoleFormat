using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public static class ElementExts
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method is for advanced cases.")]
        public static T Name<T>([NotNull] this T @this, [NotNull] out T element)
            where T : Element
        {
            element = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this;
        }

        [Obsolete("Use Element.Children.Add and Element.Children collection initializer instead.")]
        public static T AddChildren<T>([NotNull] this T @this, [NotNull] params object[] children)
            where T : Element
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            if (children == null)
                throw new ArgumentNullException(nameof(children));
            @this.Children.Add(children);
            return @this;
        }

        [Obsolete("Use Element[] and Element.Values collection initializer instead.")]
        public static T Set<T, TValue>([NotNull] this T @this, [NotNull] AttachedProperty<TValue> property, TValue value)
            where T : Element, new()
        {
            (@this ?? throw new ArgumentNullException(nameof(@this))).Values.Add(property, value);
            return @this;
        }
    }
}