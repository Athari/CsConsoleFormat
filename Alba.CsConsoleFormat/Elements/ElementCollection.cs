using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentWrapper (typeof(Span))]
    public class ElementCollection : Collection<Element>, IList
    {
        private readonly Element _parent;

        public ElementCollection (Element parent)
        {
            _parent = parent;
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

        private int AddElement (Element el)
        {
            Add(el);
            el.Parent = _parent;
            return Count - 1;
        }
    }
}