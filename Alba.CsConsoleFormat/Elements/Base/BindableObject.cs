using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xaml;
using Alba.CsConsoleFormat.Markup;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public abstract class BindableObject : IAttachedPropertyStore
    {
        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpressionBase> _getters;
        private IDictionary<AttachableMemberIdentifier, object> _attachedProperties;

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
            if (obj._getters != null)
                _getters = new Dictionary<PropertyInfo, GetExpressionBase>(obj._getters);
            if (obj._attachedProperties != null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>(obj._attachedProperties);
        }

        protected virtual BindableObject CreateInstance()
        {
            return (BindableObject)MemberwiseClone();
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        int IAttachedPropertyStore.PropertyCount => _attachedProperties?.Count ?? 0;

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.TryGetProperty(AttachableMemberIdentifier identifier, out object value)
        {
            if (_attachedProperties != null && _attachedProperties.TryGetValue(identifier, out value))
                return true;
            value = AttachedProperty.Get(identifier).DefaultValueUntyped;
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.SetProperty(AttachableMemberIdentifier identifier, object value)
        {
            EnsureAttachedPropertiesCreated();
            _attachedProperties[identifier] = value;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.RemoveProperty(AttachableMemberIdentifier identifier)
        {
            return _attachedProperties != null && _attachedProperties.Remove(identifier);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.CopyPropertiesTo(KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            _attachedProperties?.CopyTo(array, index);
        }

        public bool HasValue<T>([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return _attachedProperties != null && _attachedProperties.ContainsKey(property.Identifier);
        }

        public T GetValue<T>([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return _attachedProperties == null || !_attachedProperties.TryGetValue(property.Identifier, out object value)
                ? property.DefaultValue
                : (T)value;
        }

        public void SetValue<T>([NotNull] AttachedProperty<T> property, T value)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            EnsureAttachedPropertiesCreated();
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

        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "It is .NET Framework interface.")]
        public object this[[NotNull] AttachedProperty property]
        {
            get
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                return _attachedProperties == null || !_attachedProperties.TryGetValue(property.Identifier, out object value)
                    ? property.DefaultValueUntyped
                    : value;
            }
            set
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                EnsureAttachedPropertiesCreated();
                _attachedProperties[property.Identifier] = value;
            }
        }
    }
}