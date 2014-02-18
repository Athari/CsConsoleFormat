using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("Children")]
    public abstract class Container : Element
    {
        private ElementCollection _children;

        public ElementCollection Children
        {
            get { return _children ?? (_children = new ElementCollection()); }
        }
    }
}