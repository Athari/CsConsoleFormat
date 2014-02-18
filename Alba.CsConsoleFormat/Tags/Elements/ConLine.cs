using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConLine : XElement
    {
        public ConLine () : base(TagNames.Line)
        {}

        public ConLine (params object[] content) : base(TagNames.Line, content)
        {}
    }
}