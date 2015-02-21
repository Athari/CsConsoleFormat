using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Framework.Text;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    [RuntimeNameProperty ("Name"), ContentProperty ("Children"), XmlLangProperty ("Language"), UsableDuringInitialization (true)]
    public abstract class Element : ISupportInitialize, IAttachedPropertyStore
    {
        private const ConsoleColor DefaultColor = ConsoleColor.White;
        private const ConsoleColor DefaultBgColor = ConsoleColor.Black;

        private object _dataContext;
        private IDictionary<PropertyInfo, GetExpressionBase> _getters;
        private IDictionary<AttachableMemberIdentifier, object> _attachedProperties;
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

        internal ConsoleColor EffectiveColor
        {
            get
            {
                Element elWithColor = Parents.FirstOrDefault(e => e.Color != null);
                // ReSharper disable once PossibleInvalidOperationException
                return elWithColor != null ? elWithColor.Color.Value : DefaultColor;
            }
        }

        internal ConsoleColor EffectiveBgColor
        {
            get
            {
                Element elWithColor = Parents.FirstOrDefault(e => e.BgColor != null);
                // ReSharper disable once PossibleInvalidOperationException
                return elWithColor != null ? elWithColor.BgColor.Value : DefaultBgColor;
            }
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
                // Add inline child.
                if (inlineEl != null) {
                    // Add inline child to inline element.
                    if (this is InlineElement) {
                        VisualChildren.Add(inlineEl);
                    }
                    // Add inline child to block element.
                    else {
                        // Group inline children into single inline container.
                        if (inlines == null) {
                            inlines = new InlineContainer((BlockElement)this);
                            children.Add(inlines);
                        }
                        inlines.Children.Add(inlineEl);
                    }
                }
                // Add block or generator child.
                else {
                    if (inlines != null) {
                        // Close inline container.
                        inlines.VisualChildren = inlines.Children.ToList();
                        inlines = null;
                    }
                    children.Add(el);
                }
                // Recurse.
                el.GenerateVisualTree();
            }
            // Close inline container (if not closed within foreach).
            if (inlines != null) {
                inlines.VisualChildren = inlines.Children.ToList();
            }
            // Let element decide how to add children: directly or grouped into stack.
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

        public virtual IEnumerable<Element> GetVisualElements ()
        {
            yield return this;
        }

        private void UpdateDataContext ()
        {
            if (_getters == null)
                return;
            foreach (KeyValuePair<PropertyInfo, GetExpressionBase> getter in _getters)
                getter.Key.SetValue(this, getter.Value.GetValue(this));
        }

        public void Bind (PropertyInfo prop, GetExpressionBase getter)
        {
            if (_getters == null)
                _getters = new SortedList<PropertyInfo, GetExpressionBase>();
            _getters[prop] = getter;
        }

        public Element Clone ()
        {
            Element clone = CreateInstance();
            clone.CloneOverride(this);
            return clone;
        }

        protected virtual void CloneOverride (Element source)
        {
            if (HasChildren) {
                _children = new ElementCollection(source);
                foreach (Element child in source._children) {
                    Element childClone = child.Clone();
                    childClone.DataContext = null;
                    _children.Add(childClone);
                }
            }
        }

        protected virtual Element CreateInstance ()
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

        int IAttachedPropertyStore.PropertyCount
        {
            get { return _attachedProperties != null ? _attachedProperties.Count : 0; }
        }

        bool IAttachedPropertyStore.TryGetProperty (AttachableMemberIdentifier identifier, out object value)
        {
            if (_attachedProperties == null || !_attachedProperties.TryGetValue(identifier, out value)) {
                value = AttachedProperty.Get(identifier).DefaultValueUntyped;
                return false;
            }
            return true;
        }

        void IAttachedPropertyStore.SetProperty (AttachableMemberIdentifier identifier, object value)
        {
            if (_attachedProperties == null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>();
            _attachedProperties[identifier] = value;
        }

        bool IAttachedPropertyStore.RemoveProperty (AttachableMemberIdentifier identifier)
        {
            return _attachedProperties != null && _attachedProperties.Remove(identifier);
        }

        void IAttachedPropertyStore.CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            if (_attachedProperties == null)
                return;
            _attachedProperties.CopyTo(array, index);
        }

        public T GetValue<T> (AttachedProperty<T> property)
        {
            object value;
            return _attachedProperties == null || !_attachedProperties.TryGetValue(property.Identifier, out value)
                ? property.DefaultValue : (T)value;
        }

        public void SetValue<T> (AttachedProperty<T> property, T value)
        {
            if (_attachedProperties == null)
                _attachedProperties = new ConcurrentDictionary<AttachableMemberIdentifier, object>();
            _attachedProperties[property.Identifier] = value;
        }

        public void ResetValue<T> (AttachedProperty<T> property)
        {
            if (_attachedProperties == null)
                return;
            _attachedProperties.Remove(property.Identifier);
        }

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