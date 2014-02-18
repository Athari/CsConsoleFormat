using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConTable : XElement
    {
        public ConTable () : base(TagNames.Table)
        {}

        public ConTable (params object[] content) : base(TagNames.Table, content)
        {}
    }
}