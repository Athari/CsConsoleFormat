using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Xaml;
using JetBrains.Annotations;

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
    }

    public sealed class AttachedProperty<T> : AttachedProperty
    {
        [CanBeNull]
        public T DefaultValue => (T)DefaultValueUntyped;

        internal AttachedProperty([NotNull] AttachableMemberIdentifier identifier, T defaultValue) : base(identifier, defaultValue)
        { }
    }
}