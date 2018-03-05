using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Windows.Markup;
#elif PORTABLE_XAML
using Portable.Xaml.Markup;
#endif

namespace Alba.CsConsoleFormat
{
    #if XAML
    [ContentWrapper(typeof(Span))]
    #endif
    //[WhitespaceSignificantCollection]
    public abstract class ElementCollection<T> : Collection<T>, IList
        where T : Element
    {
        [CanBeNull]
        protected Element Parent { get; }

        protected ElementCollection(Element parent)
        {
            Parent = parent;
        }

        /// <summary>Add node through XAML.</summary>
        int IList.Add(object value)
        {
            switch (value) {
                case string text:
                    if (!(new Span(text) is T span))
                        throw new ArgumentException("Text cannot be added.");
                    AddItem(span);
                    break;
                case T el:
                    AddItem(el);
                    break;
                default:
                    if (typeof(T).IsAssignableFrom(typeof(Span)))
                        throw new ArgumentException($"Only {typeof(T).Name} and string can be added.", nameof(value));
                    else
                        throw new ArgumentException($"Only {typeof(T).Name} can be added.", nameof(value));
            }
            return Count - 1;
        }

        /// <summary>Add node through collection initializer.</summary>
        public virtual void Add(object child)
        {
            if (child == null)
                return;
            AddItem(child as T ?? throw new ArgumentException($"Only {typeof(T).Name} can be added."));
        }

        /// <summary>Add node through simple sequental code.</summary>
        public void Add(params object[] children)
        {
            Add((object)children);
        }

        [SuppressMessage("ReSharper", "AnnotationRedundancyInHierarchy", Justification = "Base Collection is not supposed to contain only non-null items.")]
        protected override void InsertItem(int index, [NotNull] T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            item.Parent = Parent;
          #if XAML
            if (item.DataContext == null && Parent != null)
                item.DataContext = Parent.DataContext;
          #endif
            base.InsertItem(index, item);
        }

        protected void AddItem(T item)
        {
            InsertItem(Count, item);
        }
    }

    public class ElementCollection : ElementCollection<Element>
    {
        public ElementCollection(Element parent) : base(parent)
        { }

        public override void Add(object child)
        {
            switch (child) {
                case null:
                    break;
                case IEnumerable enumerable when !(enumerable is string):
                    foreach (object subchild in enumerable)
                        Add(subchild);
                    break;
                case string text:
                    AddItem(new Span(text));
                    break;
                case Element element:
                    AddItem(element);
                    break;
                case IFormattable formattable:
                    AddItem(new Span(formattable.ToString(null, Parent?.EffectiveCulture)));
                    break;
                default:
                    AddItem(new Span(child.ToString()));
                    break;
            }
        }
    }
}