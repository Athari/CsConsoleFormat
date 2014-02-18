using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConSpan : XElement
    {
        public ConSpan () : base(TagNames.Span)
        {}

        public ConSpan (params object[] content) : base(TagNames.Span, content)
        {}
    }
}