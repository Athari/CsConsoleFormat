using System.Collections.Generic;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Windows.Markup;
#elif PORTABLE_XAML
using Portable.Xaml.Markup;
#endif

// TODO Add to Repeater: HeaderTpl, FooterTpl, AlternatingItemTpl, SeparatorTpl
namespace Alba.CsConsoleFormat
{
    [ContentProperty(nameof(ItemTemplate))]
    public class Repeater : GeneratorElement
    {
        private ElementCollection _itemTemplate;

        [CanBeNull]
        public IEnumerable<object> Items { get; set; }

        public ElementCollection ItemTemplate => _itemTemplate ?? (_itemTemplate = new ElementCollection(null));

        public override IEnumerable<Element> GenerateVisualElements()
        {
            if (Items == null || _itemTemplate == null)
                yield break;
            foreach (object item in Items) {
                foreach (Element element in _itemTemplate) {
                    var elementClone = (Element)element.Clone();
                    elementClone.DataContext = item;
                    foreach (Element visualElement in elementClone.GenerateVisualElements())
                        yield return visualElement;
                }
            }
        }

        public override string ToString() => base.ToString() + $" ItemTpl={_itemTemplate?.Count ?? 0}";
    }
}