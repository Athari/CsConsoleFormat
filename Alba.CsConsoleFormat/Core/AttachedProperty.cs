using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xaml;

namespace Alba.CsConsoleFormat
{
    public class AttachedProperty
    {
        private const string PropertySuffix = "Property";

        private static readonly Dictionary<AttachableMemberIdentifier, AttachedProperty> _Properties =
            new Dictionary<AttachableMemberIdentifier, AttachedProperty>();

        internal AttachableMemberIdentifier Identifier { get; private set; }
        internal object DefaultValueUntyped { get; private set; }

        internal AttachedProperty (AttachableMemberIdentifier identifier, object defaultValue)
        {
            Identifier = identifier;
            DefaultValueUntyped = defaultValue;
        }

        public string Name
        {
            get { return Identifier.MemberName; }
        }

        public Type OwnerType
        {
            get { return Identifier.DeclaringType; }
        }

        public static AttachedProperty<T> Register<TOwner, T> (string name, T defaultValue = default(T))
        {
            var identifier = new AttachableMemberIdentifier(typeof(TOwner), name);
            var property = new AttachedProperty<T>(identifier, defaultValue);
            _Properties.Add(identifier, property);
            return property;
        }

        public static AttachedProperty<T> Register<TOwner, T> (Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T))
        {
            string name = "";
            var memberExpr = nameExpression.Body as MemberExpression;
            if (memberExpr != null)
                name = memberExpr.Member.Name;
            if (name.EndsWith(PropertySuffix, StringComparison.Ordinal))
                name = name.Substring(0, name.Length - PropertySuffix.Length);
            return Register<TOwner, T>(name, defaultValue);
        }

        internal static AttachedProperty Get (AttachableMemberIdentifier identifier)
        {
            return _Properties[identifier];
        }
    }

    public class AttachedProperty<T> : AttachedProperty
    {
        public T DefaultValue
        {
            get { return (T)DefaultValueUntyped; }
        }

        internal AttachedProperty (AttachableMemberIdentifier identifier, T defaultValue) : base(identifier, defaultValue)
        {}
    }
}