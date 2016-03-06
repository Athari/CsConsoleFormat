using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public static class ElementExts
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method is for advanced cases.")]
        public static T Name<T>(this T @this, out T element)
            where T : Element
        {
            element = @this;
            return @this;
        }

        public static T AddChildren<T>(this T @this, params object[] children)
            where T : Element
        {
            foreach (object child in children) {
                if (child == null)
                    continue;
                var enumerable = child as IEnumerable;
                if (enumerable != null && !(enumerable is string)) {
                    foreach (object subchild in enumerable)
                        @this.AddChildren(subchild);
                }
                else {
                    @this.AddChild(child);
                }
            }
            return @this;
        }

        private static void AddChild<T>(this T @this, object child)
            where T : Element
        {
            var text = child as string;
            if (text != null) {
                @this.Children.Add(text);
                return;
            }
            var element = child as Element;
            if (element != null) {
                @this.Children.Add(element);
                return;
            }
            var formattable = child as IFormattable;
            if (formattable != null) {
                @this.Children.Add(formattable.ToString(null, @this.EffectiveCulture));
                return;
            }
            @this.Children.Add(child.ToString());
        }

        public static T Set<T, TValue>(this T @this, AttachedProperty<TValue> property, TValue value)
            where T : Element, new()
        {
            @this.SetValue(property, value);
            return @this;
        }
    }
}