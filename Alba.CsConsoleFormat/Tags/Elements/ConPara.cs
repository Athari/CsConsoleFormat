using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConPara : XElement
    {
        public ConPara () : base(TagNames.Para)
        {}

        public ConPara (params object[] content) : base(TagNames.Para, content)
        {}
    }
}