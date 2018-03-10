using System;

namespace Alba.CsConsoleFormat.Fluent
{
    public static class ValuesInitializerFluentExts
    {
        public static void Add(this ValuesInitializer @this, Align value) =>
            @this.Element<BlockElement, Align>().Align = value;

        public static void Add(this ValuesInitializer @this, ConsoleColor value) =>
            @this.Element<Element, Align>().Color = value;

        public static void Add(this ValuesInitializer @this, ConsoleColorExts.ConsoleColorOnBackground value)
        {
            @this.Element<Element, ConsoleColorExts.ConsoleColorOnBackground>().Color = value.Color;
            @this.Element<Element, ConsoleColorExts.ConsoleColorOnBackground>().Background = value.Background;
        }

        public static void Add(this ValuesInitializer @this, DockTo value) =>
            @this.Element<BlockElement, DockTo>().SetValue(Dock.ToProperty, value);

        public static void Add(this ValuesInitializer @this, GridLength value) =>
            @this.Element<Column, GridLength>().Width = value;

        public static void Add(this ValuesInitializer @this, TextAlign value) =>
            @this.Element<BlockElement, TextAlign>().TextAlign = value;

        public static void Add(this ValuesInitializer @this, TextWrap value) =>
            @this.Element<BlockElement, TextWrap>().TextWrap = value;

        public static void Add(this ValuesInitializer @this, VerticalAlign value) =>
            @this.Element<BlockElement, VerticalAlign>().VerticalAlign = value;

        public static void Add(this ValuesInitializer @this, Visibility value) =>
            @this.Element<Element, Visibility>().Visibility = value;

        private static T Element<T, TValue>(this ValuesInitializer @this) where T : BindableObject
        {
            if (@this.Object == null)
                throw new ArgumentNullException(nameof(@this));
            else if (@this.Object is T element)
                return element;
            else
                throw new ArgumentException($"Cannot assign {typeof(TValue)} value to {@this.Object.GetType()}. {typeof(T)} element is required.", nameof(@this));
        }
    }
}