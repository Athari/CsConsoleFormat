using System;
using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConForegroundAttr : XAttribute
    {
        public ConForegroundAttr (ConsoleColor value) : base(AttrNames.Foreground, value.ToString())
        {}
    }
}