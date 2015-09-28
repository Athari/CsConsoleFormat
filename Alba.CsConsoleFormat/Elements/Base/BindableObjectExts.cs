using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    internal static class BindableObjectExts
    {
        public static bool HasValueSafe<T> (this BindableObject @this, [NotNull] AttachedProperty<T> property)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            return @this.HasValue(property);
        }

        public static T GetValueSafe<T> (this BindableObject @this, [NotNull] AttachedProperty<T> property)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            return @this.GetValue(property);
        }

        public static void SetValueSafe<T> (this BindableObject @this, [NotNull] AttachedProperty<T> property, T value)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            @this.SetValue(property, value);
        }

        public static void ResetValueSafe<T> (this BindableObject @this, [NotNull] AttachedProperty<T> property)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            @this.ResetValue(property);
        }
    }
}