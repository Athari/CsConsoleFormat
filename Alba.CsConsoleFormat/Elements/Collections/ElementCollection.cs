using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentWrapper (typeof(Span))]
    //[WhitespaceSignificantCollection]
    public class ElementCollection : Collection<Element>, IList
    {
        private readonly ContainerElement _parent;
        private readonly GeneratorElement _generator;

        public ElementCollection (ContainerElement parent, GeneratorElement generator)
        {
            _parent = parent;
            _generator = generator;
        }

        int IList.Add (object value)
        {
            var text = value as string;
            if (text != null)
                return AddText(text);
            var el = value as Element;
            if (el != null)
                return AddElement(el);
            throw new ArgumentException("Only Element and string can be added.", "value");
        }

        public int Add (string text)
        {
            return AddText(text);
        }

        protected override void InsertItem (int index, Element el)
        {
            el.Parent = _parent;
            el.Generator = _generator;
            base.InsertItem(index, el);
        }

        private int AddText (string text)
        {
            return AddElement(new Span(text));
        }

        private int AddElement (Element el)
        {
            InsertItem(Count, el);
            return Count - 1;
        }

        internal void InsertElements (int index, IEnumerable<Element> els)
        {
            int i = index;
            foreach (Element el in els) {
                Insert(i++, el);
                el.Parent = _parent;
                el.Generator = _generator;
            }
        }
    }
}