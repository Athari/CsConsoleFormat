using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderElementExts
    {
        [SuppressMessage ("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method is for advanced cases.")]
        public static ElementBuilder<T> Name<T> (this ElementBuilder<T> @this, out T element)
            where T : Element, new()
        {
            element = @this.Element;
            return @this;
        }

        public static ElementBuilder<T> Name<T> (this ElementBuilder<T> @this, string name)
            where T : Element, new()
        {
            @this.Element.Name = name;
            return @this;
        }

        public static ElementBuilder<T> AddChildren<T> (this ElementBuilder<T> @this, params object[] children)
            where T : Element, new()
        {
            foreach (object child in children) {
                if (child == null)
                    continue;
                var enumerable = child as IEnumerable;
                if (enumerable != null) {
                    foreach (object subchild in enumerable)
                        @this.AddChildren(subchild);
                }
                else {
                    @this.AddChild(child);
                }
            }
            return @this;
        }

        private static void AddChild<T> (this ElementBuilder<T> @this, object child)
            where T : Element, new()
        {
            var text = child as string;
            if (text != null) {
                @this.Element.Children.Add(text);
                return;
            }
            var element = child as Element;
            if (element != null) {
                @this.Element.Children.Add(element);
                return;
            }
            var elementBuilder = child as IElementBuilder;
            if (elementBuilder != null) {
                @this.Element.Children.Add(elementBuilder.Element);
                return;
            }
            var formattable = child as IFormattable;
            if (formattable != null) {
                @this.Element.Children.Add(formattable.ToString(null, @this.Element.EffectiveCulture));
                return;
            }
            @this.Element.Children.Add(child.ToString());
        }

        public static ElementBuilder<T> SetValue<T, TValue> (this ElementBuilder<T> @this, AttachedProperty<TValue> property, TValue value)
            where T : Element, new()
        {
            @this.Element.SetValue(property, value);
            return @this;
        }

        public static ElementBuilder<T> Color<T> (this ElementBuilder<T> @this, ConsoleColor? color = null, ConsoleColor? bgColor = null)
            where T : Element, new()
        {
            @this.Element.Color = color;
            @this.Element.BgColor = bgColor;
            return @this;
        }

        public static ElementBuilder<T> Language<T> (this ElementBuilder<T> @this, XmlLanguage language)
            where T : Element, new()
        {
            @this.Element.Language = language;
            return @this;
        }

        public static ElementBuilder<T> Language<T> (this ElementBuilder<T> @this, string language)
            where T : Element, new()
        {
            @this.Element.Language = new XmlLanguage(language);
            return @this;
        }

        public static ElementBuilder<T> Language<T> (this ElementBuilder<T> @this, CultureInfo culture)
            where T : Element, new()
        {
            @this.Element.Language = new XmlLanguage(culture);
            return @this;
        }

        public static ElementBuilder<T> Show<T> (this ElementBuilder<T> @this, Visibility visibility = Visibility.Visible)
            where T : Element, new()
        {
            @this.Element.Visibility = visibility;
            return @this;
        }

        public static ElementBuilder<T> Hide<T> (this ElementBuilder<T> @this, bool hidden = true)
            where T : Element, new()
        {
            if (hidden)
                @this.Element.Visibility = Visibility.Hidden;
            return @this;
        }

        public static ElementBuilder<T> Collapse<T> (this ElementBuilder<T> @this, bool collapse = true)
            where T : Element, new()
        {
            if (collapse)
                @this.Element.Visibility = Visibility.Hidden;
            return @this;
        }
    }
}