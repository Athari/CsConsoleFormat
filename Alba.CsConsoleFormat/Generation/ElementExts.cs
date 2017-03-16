using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

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
            foreach (object child in children ?? throw new ArgumentNullException(nameof(children))) {
                switch (child) {
                    case null:
                        continue;
                    case IEnumerable enumerable when !(enumerable is string):
                        foreach (object subchild in enumerable)
                            @this.AddChildren(subchild);
                        break;
                    default:
                        @this.AddChild(child);
                        break;
                }
            }
            return @this;
        }

        private static void AddChild<T>(this T @this, [NotNull] object child)
            where T : Element
        {
            switch (child) {
                case string text:
                    @this.Children.Add(text);
                    break;
                case Element element:
                    @this.Children.Add(element);
                    break;
                case IFormattable formattable:
                    @this.Children.Add(formattable.ToString(null, @this.EffectiveCulture));
                    break;
                default:
                    @this.Children.Add(child.ToString());
                    break;
            }
        }

        public static T Set<T, TValue>(this T @this, AttachedProperty<TValue> property, TValue value)
            where T : Element, new()
        {
            (@this ?? throw new ArgumentNullException(nameof(@this))).SetValue(property, value);
            return @this;
        }
    }
}