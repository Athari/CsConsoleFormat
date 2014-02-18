using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConUnorderedList : XElement
    {
        public ConUnorderedList () : base(TagNames.UnorderedList)
        {}

        public ConUnorderedList (params object[] content) : base(TagNames.UnorderedList, content)
        {}
    }
}