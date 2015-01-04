using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Framework.Text;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    [RuntimeNameProperty ("Name"), ContentProperty ("Children"), XmlLangProperty ("Language"), UsableDuringInitialization (true)]
    public abstract class Element : ISupportInitialize
    {
        private ElementCollection _children;
        private IDictionary<PropertyInfo, GetExpression> _getters;

        public object DataContext { get; set; }

        public string Name { get; set; }

        public XmlLanguage Language { get; set; }

        public Element Parent { get; internal set; }

        [TypeConverter (typeof(ConsoleColorConverter))]
        public ConsoleColor? Color { get; set; }

        [TypeConverter (typeof(ConsoleColorConverter))]
        public ConsoleColor? BgColor { get; set; }

        public ElementCollection Children
        {
            get
            {
                if (!CanHaveChildren)
                    throw new NotSupportedException("Element '{0}' cannot contain children.".Fmt(GetType().Name));
                return _children ?? (_children = new ElementCollection(this));
            }
        }

        protected virtual bool CanHaveChildren
        {
            get { return true; }
        }

        internal CultureInfo EffectiveCulture
        {
            get
            {
                Element elWithLng = this.TraverseList(e => e.Parent).FirstOrDefault(e => e.Language != null);
                return elWithLng != null ? elWithLng.Language.Culture : null;
            }
        }

        private void UpdateDataContext ()
        {
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpression> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue());
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
            return "{0}:{1}{2}{3}".Fmt(
                GetType().Name,
                Name != null ? " Name={0}".Fmt(Name) : "",
                " DC={0}".Fmt(DataContext ?? "null"),
                _children != null && _children.Count > 0 ? " Children={0}".Fmt(_children.Count) : "");
        }
    }
}