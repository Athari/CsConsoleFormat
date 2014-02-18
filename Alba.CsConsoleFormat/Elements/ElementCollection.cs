using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentWrapper (typeof(Span))]
    public class ElementCollection : Collection<Element>, IList
    {
        int IList.Add (object value)
        {
            var str = value as string;
            if (str != null) {
                Add(new Span(str));
                return Count - 1;
            }
            var el = value as Element;
            if (el != null) {
                Add(el);
                return Count - 1;
            }
            throw new ArgumentException("Only Element and string can be added.", "value");
        }
    }
}