using System.Collections.Generic;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("Children")]
    public abstract class Element : Base
    {
        private IList<Element> _children;

        public IList<Element> Children
        {
            get { return _children ?? (_children = new List<Element>()); }
        }
    }
}