using System.Xml.Linq;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    internal class TagNames
    {
        public static readonly XNamespace ConNs = XNamespace.Get("urn:alba:cs-console-format");

        public static readonly XName Body = GetName<ConBody>();
        public static readonly XName Line = GetName<ConLine>();
        public static readonly XName OrderedList = GetName<ConOrderedList>();
        public static readonly XName Para = GetName<ConPara>();
        public static readonly XName Span = GetName<ConSpan>();
        public static readonly XName Table = GetName<ConTable>();
        public static readonly XName UnorderedList = GetName<ConUnorderedList>();

        private static XName GetName<TConTag> () where TConTag : XElement
        {
            return ConNs.GetName(typeof(TConTag).Name.RemovePrefix("Con"));
        }
    }
}