using System.Linq;
using System.Windows.Markup;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("Children")]
    public abstract class Container : Element
    {
        private ElementCollection _children;

        public ElementCollection Children
        {
            get { return _children ?? (_children = new ElementCollection(this)); }
        }

        protected override void UpdateDataContext ()
        {
            base.UpdateDataContext();
            if (_children == null)
                return;
            foreach (Element element in _children.Where(el => el.DataContext == null))
                element.DataContext = DataContext;
        }
    }
}