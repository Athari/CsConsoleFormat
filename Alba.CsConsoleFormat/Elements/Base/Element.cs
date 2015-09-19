using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    [RuntimeNameProperty (nameof(Name)), ContentProperty (nameof(Children)), XmlLangProperty (nameof(Language)), UsableDuringInitialization (true)]
    public abstract class Element : BindableObject
    {
        private const ConsoleColor DefaultColor = ConsoleColor.White;
        private const ConsoleColor DefaultBgColor = ConsoleColor.Black;

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

        public ElementCollection Children
        {
            get
            {
                if (!CanHaveChildren)
                    throw new NotSupportedException($"Element '{(GetType().Name)}' cannot contain children.");
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

        internal BlockElement VisualChild => (BlockElement)_visualChildren?.SingleOrDefault();

        protected virtual bool CanHaveChildren => true;

        private bool HasChildren => _children != null && _children.Count > 0;

        private IEnumerable<Element> Parents => this.TraverseList(e => e.Parent);

        internal ConsoleColor EffectiveColor => Parents.FirstOrDefault(e => e.Color != null)?.Color ?? DefaultColor;
        internal ConsoleColor EffectiveBgColor => Parents.FirstOrDefault(e => e.BgColor != null)?.BgColor ?? DefaultBgColor;
        internal CultureInfo EffectiveCulture => Parents.FirstOrDefault(e => e.Language != null)?.Language.Culture;

        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public void GenerateVisualTree ()
        {
            if (!HasChildren)
                return;
            var children = new List<Element>();
            InlineContainer inlines = null;
            foreach (Element el in _children.SelectMany(c => c.GenerateVisualElements())) {
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

        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public virtual IEnumerable<Element> GenerateVisualElements ()
        {
            yield return this;
        }

        protected override void CloneOverride (BindableObject obj)
        {
            var source = (Element)obj;
            base.CloneOverride(source);
            if (HasChildren) {
                _children = new ElementCollection(source);
                foreach (Element child in source._children) {
                    var childClone = (Element)child.Clone();
                    childClone.DataContext = null;
                    _children.Add(childClone);
                }
            }
        }

        public override string ToString () =>
            $"{GetType().Name}:"
                + (Name != null ? $" Name={Name}" : "")
                + (DataContext != null ? $" DC={DataContext}" : " DC=null")
                + (HasChildren ? $" Children={_children.Count}" : "");
    }
}