using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Markup;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Windows.Markup;
#elif PORTABLE_XAML
using Portable.Xaml.Markup;
#endif

namespace Alba.CsConsoleFormat
{
    #if XAML
    [RuntimeNameProperty(nameof(Name)), ContentProperty(nameof(Children)), XmlLangProperty(nameof(Language)), UsableDuringInitialization(true)]
    #endif
    public abstract class Element : BindableObject
    {
        private const ConsoleColor DefaultColor = ConsoleColor.White;
        private const ConsoleColor DefaultBackground = ConsoleColor.Black;

        [CanBeNull, ItemNotNull]
        private ElementCollection _children;

        [CanBeNull, ItemNotNull]
        private IList<Element> _visualChildren;

        [CanBeNull]
        public string Name { get; set; }

        [CanBeNull]
        public XmlLanguage Language { get; set; }

        [CanBeNull]
        public Element Parent { get; internal set; }

        [TypeConverter(typeof(ConsoleColorConverter))]
        public ConsoleColor? Color { get; set; }

        [TypeConverter(typeof(ConsoleColorConverter))]
        public ConsoleColor? Background { get; set; }

        public Visibility Visibility { get; set; }

        protected Element()
        { }

        protected Element(params object[] children)
        {
            if (!ElementCollection.IsNullOrEmpty(children))
                Children.Add(children);
        }

        [ItemNotNull]
        public ElementCollection Children
        {
            get
            {
                if (!CanHaveChildren)
                    throw new NotSupportedException($"Element '{GetType().Name}' cannot contain children.");
                return _children ?? (_children = new ElementCollection(this));
            }
        }

        // TODO Change type of Element.VisualChildren to IList<BlockElement>
        [NotNull, ItemNotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected internal IList<Element> VisualChildren
        {
            get => _visualChildren ?? (_visualChildren = new List<Element>());
            internal set => _visualChildren = value;
        }

        [CanBeNull]
        internal BlockElement VisualChild => (BlockElement)_visualChildren?.SingleOrDefault();

        protected virtual bool CanHaveChildren => true;

        private IEnumerable<Element> Parents => this.TraverseList(e => e.Parent);

        protected internal ConsoleColor EffectiveColor => Parents.FirstOrDefault(e => e.Color != null)?.Color ?? DefaultColor;
        protected internal ConsoleColor EffectiveBackground => Parents.FirstOrDefault(e => e.Background != null)?.Background ?? DefaultBackground;
        protected internal CultureInfo EffectiveCulture => Parents.FirstOrDefault(e => e.Language != null)?.Language?.Culture ?? CultureInfo.CurrentCulture;

        #if XAML
        protected override void UpdateDataContext()
        {
            base.UpdateDataContext();
            if (_children == null)
                return;
            foreach (Element child in _children.Where(c => c.DataContext == null))
                child.DataContext = DataContext;
        }
        #endif

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void GenerateVisualTree()
        {
            if (!(_children?.Count > 0))
                return;
            var children = new List<Element>();
            InlineContainer inlines = null;
            foreach (Element el in _children.SelectMany(c => c.GenerateVisualElements())) {
                // Add inline child.
                if (el is InlineElement inlineEl) {
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

        protected virtual void SetVisualChildren([NotNull, ItemNotNull] IList<Element> visualChildren)
        {
            if (visualChildren == null)
                throw new ArgumentNullException(nameof(visualChildren));
            if (visualChildren.Count == 1) {
                VisualChildren = visualChildren;
            }
            else if (visualChildren.Count > 1) {
                VisualChildren = new List<Element>(1) {
                    new Stack {
                      #if XAML
                        DataContext = DataContext,
                      #endif
                        VisualChildren = visualChildren,
                    }
                };
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual IEnumerable<Element> GenerateVisualElements()
        {
            yield return this;
        }

        protected override void CloneOverride(BindableObject obj)
        {
            var source = (Element)obj;
            base.CloneOverride(source);
            if (source._children?.Count > 0) {
                _children = new ElementCollection(this);
                foreach (Element child in source._children) {
                    var childClone = (Element)child.Clone();
                  #if XAML
                    childClone.DataContext = null;
                  #endif
                    _children.Add(childClone);
                }
            }
        }

        public override string ToString() =>
            $"{GetType().Name}:"
          + (Name != null ? $" Name={Name}" : "")
          #if XAML
          + (DataContext != null ? $" DC={DataContext}" : "")
          #endif
          + (_children?.Count > 0 ? $" Children={_children.Count}" : "");
    }
}