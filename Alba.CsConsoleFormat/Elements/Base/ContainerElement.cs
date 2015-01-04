using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("Children")]
    public abstract class ContainerElement : BlockElement
    {
        private ElementCollection _children;

        public ElementCollection Children
        {
            get { return _children ?? (_children = new ElementCollection(this)); }
        }

        public override string ToString ()
        {
            return base.ToString() + " Children={0}".Fmt(_children != null ? _children.Count : 0);
        }
    }
}