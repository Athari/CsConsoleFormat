using System.Collections.Generic;
using System.Windows.Markup;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    [ContentProperty ("ItemTemplate")]
    public class Repeater : Generator
    {
        private IEnumerable<object> _items;
        private ElementCollection _itemTemplate;

        public IEnumerable<object> Items
        {
            get { return _items; }
            set
            {
                if (_items == value)
                    return;
                _items = value;
                UpdateGeneratedItems();
            }
        }

        public ElementCollection ItemTemplate
        {
            get { return _itemTemplate ?? (_itemTemplate = new ElementCollection(null, this)); }
        }

        private void UpdateGeneratedItems ()
        {
            Parent.Children.RemoveAll(el => el.Generator == this);
            if (_items == null || _itemTemplate == null)
                return;
            var generatedChildren = new List<Element>();
            foreach (object item in _items) {
                foreach (Element element in _itemTemplate) {
                    Element generatedElement = element.Clone();
                    generatedElement.DataContext = item;
                    generatedChildren.Add(generatedElement);
                }
            }
            Parent.Children.InsertElements(Parent.Children.IndexOf(this) + 1, generatedChildren);
        }

        public override string ToString ()
        {
            return base.ToString() + " ItemTpl={0}".Fmt(_itemTemplate != null ? _itemTemplate.Count : 0);
        }
    }
}