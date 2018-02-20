using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Consistent API.")]
    internal static class BindableObjectExts
    {
        [Pure]
        public static bool HasValueSafe<T>([NotNull] this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).HasValue(property ?? throw new ArgumentException(nameof(property)));

        [Pure, CanBeNull]
        public static T GetValueSafe<T>([NotNull] this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).GetValue(property ?? throw new ArgumentException(nameof(property)));

        public static void SetValueSafe<T>([NotNull] this BindableObject @this, [NotNull] AttachedProperty<T> property, T value) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).SetValue(property ?? throw new ArgumentException(nameof(property)), value);

        public static void ResetValueSafe<T>([NotNull] this BindableObject @this, [NotNull] AttachedProperty<T> property) =>
            (@this ?? throw new ArgumentNullException(nameof(@this))).ResetValue(property ?? throw new ArgumentException(nameof(property)));
    }
}