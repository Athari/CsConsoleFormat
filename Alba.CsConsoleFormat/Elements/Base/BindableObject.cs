using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xaml;
using Alba.CsConsoleFormat.Markup;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class BindableObject : ISupportInitialize, IAttachedPropertyStore
    {
        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpressionBase> _getters;
        private IDictionary<AttachableMemberIdentifier, object> _attachedProperties;

        [CanBeNull]
        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                if (_dataContext == value)
                    return;
                _dataContext = value;
                UpdateDataContext();
            }
        }

        private void UpdateDataContext ()
        {
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpressionBase> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue(this), null);
        }

        public void Bind ([NotNull] PropertyInfo prop, [NotNull] GetExpressionBase getter)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            if (getter == null)
                throw new ArgumentNullException(nameof(getter));
            if (_getters == null)
                _getters = new SortedList<PropertyInfo, GetExpressionBase>();
            _getters[prop] = getter;
        }

        public BindableObject Clone ()
        {
            BindableObject clone = CreateInstance();
            clone.CloneOverride(this);
            return clone;
        }

        protected virtual void CloneOverride (BindableObject obj)
        {}

        protected virtual BindableObject CreateInstance ()
        {
            return (BindableObject)MemberwiseClone();
        }

        void ISupportInitialize.BeginInit ()
        {
            BeginInit();
        }

        void ISupportInitialize.EndInit ()
        {
            EndInit();
        }

        protected virtual void BeginInit ()
        {}

        protected virtual void EndInit ()
        {}

        [SuppressMessage ("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        int IAttachedPropertyStore.PropertyCount => _attachedProperties?.Count ?? 0;

        [SuppressMessage ("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.TryGetProperty (AttachableMemberIdentifier identifier, out object value)
        {
            if (_attachedProperties != null && _attachedProperties.TryGetValue(identifier, out value))
                return true;
            value = AttachedProperty.Get(identifier).DefaultValueUntyped;
            return false;
        }

        [SuppressMessage ("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.SetProperty (AttachableMemberIdentifier identifier, object value)
        {
            EnsureAttachedPropertiesCreated();
            _attachedProperties[identifier] = value;
        }

        [SuppressMessage ("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        bool IAttachedPropertyStore.RemoveProperty (AttachableMemberIdentifier identifier)
        {
            return _attachedProperties != null && _attachedProperties.Remove(identifier);
        }

        [SuppressMessage ("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IAttachedPropertyStore should not be reimplemented.")]
        void IAttachedPropertyStore.CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            _attachedProperties?.CopyTo(array, index);
        }

        public bool HasValue<T> ([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return _attachedProperties != null && _attachedProperties.ContainsKey(property.Identifier);
        }

        public T GetValue<T> ([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            object value;
            return _attachedProperties == null || !_attachedProperties.TryGetValue(property.Identifier, out value)
                ? property.DefaultValue : (T)value;
        }

        public void SetValue<T> ([NotNull] AttachedProperty<T> property, T value)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            EnsureAttachedPropertiesCreated();
            _attachedProperties[property.Identifier] = value;
        }

        public void ResetValue<T> ([NotNull] AttachedProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            _attachedProperties?.Remove(property.Identifier);
        }

        private void EnsureAttachedPropertiesCreated ()
        {
            if (_attachedProperties == null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>();
        }

        public object this [[NotNull] AttachedProperty property]
        {
            get
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                object value;
                return _attachedProperties == null || !_attachedProperties.TryGetValue(property.Identifier, out value)
                    ? property.DefaultValueUntyped : value;
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