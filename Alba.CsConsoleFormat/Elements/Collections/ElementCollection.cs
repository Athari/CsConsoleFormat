using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentWrapper (typeof(Span))]
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
            var str = value as string;
            if (str != null)
                return AddElement(new Span(str));
            var el = value as Element;
            if (el != null)
                return AddElement(el);
            throw new ArgumentException("Only Element and string can be added.", "value");
        }

        public int AddElement (Element el)
        {
            Add(el);
            el.Parent = _parent;
            el.Generator = _generator;
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