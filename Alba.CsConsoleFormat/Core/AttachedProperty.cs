using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Xaml;
#else
using Portable.Xaml;
#endif

namespace Alba.CsConsoleFormat
{
    public class AttachedProperty
    {
        private const string PropertySuffix = "Property";

        private static readonly Dictionary<AttachableMemberIdentifier, AttachedProperty> _Properties =
            new Dictionary<AttachableMemberIdentifier, AttachedProperty>();

        internal AttachableMemberIdentifier Identifier { get; }

        [CanBeNull]
        internal object DefaultValueUntyped { get; }

        internal AttachedProperty([NotNull] AttachableMemberIdentifier identifier, object defaultValue)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            DefaultValueUntyped = defaultValue;
        }

        public string Name => Identifier.MemberName;
        public Type OwnerType => Identifier.DeclaringType;

        [Pure]
        internal static AttachedProperty Get([NotNull] AttachableMemberIdentifier identifier) =>
            _Properties[identifier ?? throw new ArgumentNullException(nameof(identifier))];

        [MustUseReturnValue]
        public static AttachedProperty<T> Register<TOwner, T>([NotNull] string name, T defaultValue = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            var identifier = new AttachableMemberIdentifier(typeof(TOwner), name);
            var property = new AttachedProperty<T>(identifier, defaultValue);
            if (_Properties.ContainsKey(identifier))
                throw new ArgumentException($"Property '{name}' already registered for type '{typeof(TOwner).Name}'.");
            _Properties.Add(identifier, property);
            return property;
        }

        [MustUseReturnValue]
        public static AttachedProperty<T> Register<TOwner, T>([NotNull] Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default)
        {
            if (nameExpression == null)
                throw new ArgumentNullException(nameof(nameExpression));
            string name = "";
            if (nameExpression.Body is MemberExpression memberExpr)
                name = memberExpr.Member.Name;
            if (name.EndsWith(PropertySuffix, StringComparison.Ordinal))
                name = name.Substring(0, name.Length - PropertySuffix.Length);
            return Register<TOwner, T>(name, defaultValue);
        }

        public bool Equals(AttachedProperty other) => Identifier == other?.Identifier;
        public override bool Equals(object obj) => Equals(obj as AttachedProperty);
        public override int GetHashCode() => Identifier.GetHashCode();
    }

    public sealed class AttachedProperty<T> : AttachedProperty
    {
        [CanBeNull]
        public T DefaultValue => (T)DefaultValueUntyped;

        internal AttachedProperty([NotNull] AttachableMemberIdentifier identifier, T defaultValue) : base(identifier, defaultValue)
        { }

        public static AttachedValue<T> operator ==(AttachedProperty<T> property, T value) => new AttachedValue<T>(property, value);
        public static AttachedValue<T> operator !=(AttachedProperty<T> property, T value) => throw new NotSupportedException();

        // ReSharper disable RedundantOverriddenMember - Either redundant overrides or compiler warnings, I choose redundant overrides.

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        // ReSharper restore RedundantOverriddenMember
    }
}