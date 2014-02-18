using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConOrderedList : XElement
    {
        public ConOrderedList () : base(TagNames.OrderedList)
        {}

        public ConOrderedList (params object[] content) : base(TagNames.OrderedList, content)
        {}
    }
}