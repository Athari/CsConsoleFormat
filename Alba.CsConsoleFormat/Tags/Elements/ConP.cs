using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConP : XElement
    {
        public ConP () : base(TagNames.P)
        {}

        public ConP (params object[] content) : base(TagNames.P, content)
        {}
    }
}