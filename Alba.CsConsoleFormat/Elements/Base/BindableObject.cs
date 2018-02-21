using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Xaml;
#else
using Portable.Xaml;
#endif

namespace Alba.CsConsoleFormat
{
    public abstract class BindableObject
        #if XAML
        : IAttachedPropertyStore
        #endif
    {
        private object _dataContext;

        #if XAML
        [CanBeNull]
        private IDictionary<PropertyInfo, GetExpressionBase> _getters;
        #endif

        [CanBeNull]
        private IDictionary<AttachableMemberIdentifier, object> _attachedProperties;

        #if XAML

        [CanBeNull]
        public object DataContext
        {
            get => _dataContext;
            set
            {
                if (_dataContext == value)
                    return;
                _dataContext = value;
                UpdateDataContext();
            }
        }

        protected virtual void UpdateDataContext()
        {
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpressionBase> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue(this), null);
        }

        public void Bind([NotNull] PropertyInfo property, [NotNull] GetExpressionBase getter)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (getter == null)
                throw new ArgumentNullException(nameof(getter));
            if (_getters == null)
                _getters = new Dictionary<PropertyInfo, GetExpressionBase>();
            getter.TargetObject = this;
            getter.TargetType = property.PropertyType;
            _getters[property] = getter;
        }

        #endif

        [Pure]
        public BindableObject Clone()
        {
            BindableObject clone = CreateInstance();
            clone.CloneOverride(this);
            return clone;
        }

        protected virtual void CloneOverride([NotNull] BindableObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
          #if XAML
            if (obj._getters != null)
                _getters = new Dictionary<PropertyInfo, GetExpressionBase>(obj._getters);
          #endif
            if (obj._attachedProperties != null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>(obj._attachedProperties);
        }

        [Pure]
        protected virtual BindableObject CreateInstance()
        {
            return (BindableObject)MemberwiseClone();
        }

        #if XAML

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        int IAttachedPropertyStore.PropertyCount => _attachedProperties?.Count ?? 0;

        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.TryGetProperty([NotNull] AttachableMemberIdentifier identifier, out object value)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            if (_attachedProperties != null && _attachedProperties.TryGetValue(identifier, out value))
                return true;
            value = AttachedProperty.Get(identifier).DefaultValueUntyped;
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.SetProperty([NotNull] AttachableMemberIdentifier identifier, object value)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            EnsureAttachedPropertiesCreated(); // ReSharper disable once PossibleNullReferenceException
            _attachedProperties[identifier] = value;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.RemoveProperty([NotNull] AttachableMemberIdentifier identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            return _attachedProperties?.Remove(identifier) == true;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.CopyPropertiesTo([NotNull] KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            _attachedProperties?.CopyTo(array, index);
        }

        #endif

        [Pure]
        public bool HasValue<T>([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return _attachedProperties?.ContainsKey(property.Identifier) == true;
        }

        [Pure, CanBeNull]
        public T GetValue<T>([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            object value = null;
            return _attachedProperties?.TryGetValue(property.Identifier, out value) == true
                ? (T)value
                : property.DefaultValue;
        }

        public void SetValue<T>([NotNull] AttachedProperty<T> property, T value)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            EnsureAttachedPropertiesCreated(); // ReSharper disable once PossibleNullReferenceException
            _attachedProperties[property.Identifier] = value;
        }

        public void ResetValue<T>([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            _attachedProperties?.Remove(property.Identifier);
        }

        private void EnsureAttachedPropertiesCreated()
        {
            if (_attachedProperties == null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>();
        }

        [Pure, CanBeNull]
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "It is .NET Framework interface.")]
        public object this[[NotNull] AttachedProperty property]
        {
            get
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                object value = null;
                return _attachedProperties?.TryGetValue(property.Identifier, out value) == true
                    ? value
                    : property.DefaultValueUntyped;
            }
            set
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                EnsureAttachedPropertiesCreated(); // ReSharper disable once PossibleNullReferenceException
                _attachedProperties[property.Identifier] = value;
            }
        }
    }
}