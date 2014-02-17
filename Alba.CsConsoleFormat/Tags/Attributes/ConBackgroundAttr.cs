using System;
using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConBackgroundAttr : XAttribute
    {
        public ConBackgroundAttr (ConsoleColor value) : base(AttrNames.Background, value.ToString())
        {}
    }
}