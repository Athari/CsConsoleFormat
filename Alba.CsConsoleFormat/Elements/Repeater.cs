using System.Collections.Generic;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Text;

// TODO Add to Repeater: HeaderTpl, FooterTpl, AlternatingItemTpl, SeparatorTpl
namespace Alba.CsConsoleFormat
{
    [ContentProperty ("ItemTemplate")]
    public class Repeater : GeneratorElement
    {
        private ElementCollection _itemTemplate;

        public IEnumerable<object> Items { get; set; }

        public ElementCollection ItemTemplate
        {
            get { return _itemTemplate ?? (_itemTemplate = new ElementCollection(null)); }
        }

        protected override IEnumerable<Element> GetVisualElements ()
        {
            if (Items == null || _itemTemplate == null)
                yield break;
            foreach (object item in Items) {
                foreach (Element element in _itemTemplate) {
                    Element generatedEl = element.Clone();
                    generatedEl.DataContext = item;
                    yield return generatedEl;
                }
            }
        }

        public override string ToString ()
        {
            return base.ToString() + " ItemTpl={0}".Fmt(_itemTemplate != null ? _itemTemplate.Count : 0);
        }
    }
}