using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public abstract class ContainerElement : BlockElement
    {
        protected override void SetVisualChildren (IList<Element> visualChildren)
        {
            VisualChildren = visualChildren;
        }
    }
}