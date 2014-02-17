using System.Xml.Linq;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    internal class TagNames
    {
        public static readonly XNamespace ConNs = XNamespace.Get("urn:alba:cs-console-format");

        public static readonly XName Body = GetName<ConBody>();
        public static readonly XName P = GetName<ConP>();

        private static XName GetName<TConTag> () where TConTag : XElement
        {
            return ConNs.GetName(typeof(TConTag).Name.RemovePrefix("Con"));
        }
    }
}