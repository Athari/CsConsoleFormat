using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConVerticalAlignAttr : XAttribute
    {
        public static readonly ConVerticalAlignAttr Top = new ConVerticalAlignAttr(VerticalAlignment.Top);
        public static readonly ConVerticalAlignAttr Center = new ConVerticalAlignAttr(VerticalAlignment.Center);
        public static readonly ConVerticalAlignAttr Bottom = new ConVerticalAlignAttr(VerticalAlignment.Bottom);
        public static readonly ConVerticalAlignAttr Justify = new ConVerticalAlignAttr(VerticalAlignment.Justify);

        public ConVerticalAlignAttr (VerticalAlignment value) : base(AttrNames.VerticalAlign, value)
        {}
    }
}