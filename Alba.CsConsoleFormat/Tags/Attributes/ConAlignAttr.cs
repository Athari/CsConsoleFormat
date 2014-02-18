using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConAlignAttr : XAttribute
    {
        public static readonly ConAlignAttr Left = new ConAlignAttr(HorizontalAlignment.Left);
        public static readonly ConAlignAttr Center = new ConAlignAttr(HorizontalAlignment.Center);
        public static readonly ConAlignAttr Right = new ConAlignAttr(HorizontalAlignment.Right);
        public static readonly ConAlignAttr Justify = new ConAlignAttr(HorizontalAlignment.Justify);

        public ConAlignAttr (HorizontalAlignment value) : base(AttrNames.Align, value.ToString())
        {}
    }
}