using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class Document : BlockElement
    {
        public Dictionary<string, object> Resources { get; private set; }

        public Document ()
        {
            Resources = new Dictionary<string, object>();
        }
    }
}