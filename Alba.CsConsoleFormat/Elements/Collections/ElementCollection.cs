using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    [ContentWrapper(typeof(Span))]
    //[WhitespaceSignificantCollection]
    public class ElementCollection<T> : Collection<T>, IList
        where T : Element
    {
        private readonly Element _parent;

        public ElementCollection(Element parent)
        {
            _parent = parent;
        }

        int IList.Add(object value)
        {
            var text = value as string;
            if (text != null)
                return AddText(text);
            var el = value as T;
            if (el != null)
                return AddElement(el);
            if (typeof(T).IsAssignableFrom(typeof(Span)))
                throw new ArgumentException($"Only {typeof(T).Name} and string can be added.", nameof(value));
            else
                throw new ArgumentException($"Only {typeof(T).Name} can be added.", nameof(value));
        }

        public int Add(string text)
        {
            return AddText(text);
        }

        protected override void InsertItem(int index, [NotNull] T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            item.Parent = _parent;
            if (item.DataContext == null && _parent != null)
                item.DataContext = _parent.DataContext;
            base.InsertItem(index, item);
        }

        private int AddText(string text)
        {
            var el = new Span(text) as T;
            if (el == null)
                throw new ArgumentException("Text cannot be added.");
            return AddElement(el);
        }

        private int AddElement(T el)
        {
            InsertItem(Count, el);
            return Count - 1;
        }
    }

    public class ElementCollection : ElementCollection<Element>
    {
        public ElementCollection(Element parent) : base(parent)
        {}
    }
}