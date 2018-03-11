using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ValuesInitializerExts
    {
        public static void Add<T>(this ValuesInitializer @this, AttachedProperty<T> property, T value) =>
            @this.Object[property] = value;

        public static void Add<T>(this ValuesInitializer @this, AttachedValue<T> value) =>
            @this.Object[value.Property] = value.Value;
    }
}