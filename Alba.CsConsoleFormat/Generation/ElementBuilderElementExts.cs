using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat.Generation
{
    public static class ElementBuilderElementExts
    {
        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this)
            where T : Element, new()
        {
            return new ElementBuilder<T>(new T());
        }

        public static ElementBuilder<T> Create<T> (this DocumentBuilder @this, out T element)
            where T : Element, new()
        {
            return new ElementBuilder<T>(element = new T());
        }

        public static ElementBuilder<T> AddChildren<T> (this ElementBuilder<T> @this, IEnumerable<Element> children)
            where T : Element, new()
        {
            @this.Element.Children.AddRange(children);
            return @this;
        }

        public static ElementBuilder<T> AddChildren<T> (this ElementBuilder<T> @this, params Element[] children)
            where T : Element, new()
        {
            return AddChildren(@this, children.AsEnumerable());
        }

        public static ElementBuilder<T> AddChildren<T> (this ElementBuilder<T> @this, IEnumerable<object> children)
            where T : Element, new()
        {
            foreach (object child in children) {
                if (child == null)
                    continue;
                var enumerable = child as IEnumerable<object>;
                if (enumerable != null)
                    AddChildren(@this, enumerable);
                else
                    AddChild(@this, child);
            }
            return @this;
        }

        public static ElementBuilder<T> AddChildren<T> (this ElementBuilder<T> @this, params object[] children)
            where T : Element, new()
        {
            AddChildren(@this, children.AsEnumerable());
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
            var elementBuilder = child as ElementBuilder;
            if (elementBuilder != null) {
                @this.Element.Children.Add(elementBuilder.ElementUntyped);
                return;
            }
            throw new ArgumentException($"Unsupported child type: {child.GetType().Name}.");
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