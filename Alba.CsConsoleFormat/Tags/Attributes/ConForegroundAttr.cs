using System;
using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public partial class ConForegroundAttr : XAttribute
    {
        public ConForegroundAttr (ConsoleColor value) : base(AttrNames.Foreground, value.ToString())
        {}
    }
}