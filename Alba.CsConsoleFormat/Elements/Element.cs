using System.Collections.Generic;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    public abstract class Element
    {
        private Element _parent;
        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpression> _getters;

        public Element Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent == value)
                    return;
                _parent = value;
                if (DataContext == null)
                    DataContext = _parent.DataContext;
            }
        }

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

        protected virtual void UpdateDataContext ()
        {
            foreach (KeyValuePair<PropertyInfo, GetExpression> getter in Getters) {
                if (getter.Value.Source == null)
                    getter.Value.Source = _dataContext;
                getter.Key.SetValue(this, getter.Value.GetValue());
            }
        }

        public IDictionary<PropertyInfo, GetExpression> Getters
        {
            get { return _getters ?? (_getters = new SortedList<PropertyInfo, GetExpression>()); }
        }
    }
}