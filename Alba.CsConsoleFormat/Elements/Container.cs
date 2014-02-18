using System.Linq;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("Children")]
    public abstract class Container : Element
    {
        private ElementCollection _children;

        public ElementCollection Children
        {
            get { return _children ?? (_children = new ElementCollection(this, null)); }
        }

        protected override void UpdateDataContext ()
        {
            base.UpdateDataContext();
            if (_children == null)
                return;
            foreach (Element element in _children.Where(el => el.DataContext == null).ToList())
                element.DataContext = DataContext;
        }

        public override string ToString ()
        {
            return base.ToString() + " Children={0}".Fmt(_children != null ? _children.Count : 0);
        }
    }
}