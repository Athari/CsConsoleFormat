using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Text;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    [RuntimeNameProperty ("Name"), XmlLangProperty ("Language"), UsableDuringInitialization (true)]
    public abstract class Element : ISupportInitialize
    {
        private ContainerElement _parent;
        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpression> _getters;

        internal GeneratorElement Generator { get; set; }

        public string Name { get; set; }
        public string Language { get; set; }

        public ContainerElement Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent == value)
                    return;
                _parent = value;
                if (Generator == null)
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
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpression> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue(_dataContext));
        }

        public void Bind (PropertyInfo prop, GetExpression getter)
        {
            if (_getters == null)
                _getters = new SortedList<PropertyInfo, GetExpression>();
            _getters[prop] = getter;
        }

        public Element Clone ()
        {
            return (Element)MemberwiseClone();
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

        public override string ToString ()
        {
            return "{0}:{1} DC={2}".Fmt(GetType().Name, Name != null ? " Name={0}".Fmt(Name) : "", DataContext);
        }
    }
}