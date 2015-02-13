using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpression> _getters;
        private ElementCollection _children;
        private IList<Element> _visualChildren;

        public string Name { get; set; }

        public XmlLanguage Language { get; set; }

        public Element Parent { get; internal set; }

        [TypeConverter (typeof(ConsoleColorConverter))]
        public ConsoleColor? Color { get; set; }

        [TypeConverter (typeof(ConsoleColorConverter))]
        public ConsoleColor? BgColor { get; set; }

        public Visibility Visibility { get; set; }

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

        public ElementCollection Children
        {
            get
            {
                if (!CanHaveChildren)
                    throw new NotSupportedException("Element '{0}' cannot contain children.".Fmt(GetType().Name));
                return _children ?? (_children = new ElementCollection(this));
            }
        }

        // TODO Change type of Element.VisualChildren to IList<BlockElement>
        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        protected internal IList<Element> VisualChildren
        {
            get { return _visualChildren ?? (_visualChildren = new List<Element>()); }
            internal set { _visualChildren = value; }
        }

        internal BlockElement VisualChild
        {
            get { return _visualChildren != null ? (BlockElement)_visualChildren.SingleOrDefault() : null; }
        }

        protected virtual bool CanHaveChildren
        {
            get { return true; }
        }

        private bool HasChildren
        {
            get { return _children != null && _children.Count > 0; }
        }

        private IEnumerable<Element> Parents
        {
            get { return this.TraverseList(e => e.Parent); }
        }

        internal CultureInfo EffectiveCulture
        {
            get
            {
                Element elWithLng = Parents.FirstOrDefault(e => e.Language != null);
                return elWithLng != null ? elWithLng.Language.Culture : null;
            }
        }

        public void GenerateVisualTree ()
        {
            if (!HasChildren)
                return;
            var children = new List<Element>();
            InlineContainer inlines = null;
            foreach (Element el in _children.SelectMany(c => c.GetVisualElements())) {
                var inlineEl = el as InlineElement;
                if (inlineEl != null) {
                    if (inlines == null) {
                        BlockElement parentBlockEl = Parents.OfType<BlockElement>().First();
                        inlines = new InlineContainer { DataContext = DataContext, TextAlign = parentBlockEl.TextAlign };
                        children.Add(inlines);
                    }
                    inlines.Children.Add(inlineEl);
                }
                else {
                    if (inlines != null) {
                        inlines.VisualChildren = inlines.Children.ToList();
                        inlines = null;
                    }
                    children.Add(el);
                }
                el.GenerateVisualTree();
            }
            if (inlines != null) {
                inlines.VisualChildren = inlines.Children.ToList();
            }
            SetVisualChildren(children);
        }

        protected virtual void SetVisualChildren (IList<Element> visualChildren)
        {
            if (visualChildren.Count == 1) {
                VisualChildren = visualChildren;
            }
            else if (visualChildren.Count > 1) {
                VisualChildren = new List<Element>(1) {
                    new Stack {
                        DataContext = DataContext,
                        VisualChildren = visualChildren,
                    }
                };
            }
        }

        protected virtual IEnumerable<Element> GetVisualElements ()
        {
            yield return this;
        }

        private void UpdateDataContext ()
        {
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpression> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue(this));
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
                HasChildren ? " Children={0}".Fmt(_children.Count) : "");
        }
    }
}