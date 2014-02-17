using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConBody : XElement
    {
        public ConBody () : base(TagNames.Body)
        {}

        public ConBody (params object[] content) : base(TagNames.Body, content)
        {}
    }
}