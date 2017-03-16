using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    internal static class BindableObjectExts
    {
        public static bool HasValueSafe<T>(this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).HasValue(property);

        public static T GetValueSafe<T>(this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).GetValue(property);

        public static void SetValueSafe<T>(this BindableObject @this, [NotNull] AttachedProperty<T> property, T value) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).SetValue(property, value);

        public static void ResetValueSafe<T>(this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).ResetValue(property);
    }
}