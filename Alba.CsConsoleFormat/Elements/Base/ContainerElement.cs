using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public abstract class ContainerElement : BlockElement
    {
        protected ContainerElement()
        { }

        protected ContainerElement(params object[] children) : base(children)
        { }

        protected override void SetVisualChildren(IList<Element> visualChildren)
        {
            VisualChildren = visualChildren;
        }
    }
}