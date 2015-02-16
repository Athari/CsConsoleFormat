using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class Document : Div
    {
        public Dictionary<string, object> Resources { get; private set; }

        public Document ()
        {
            Resources = new Dictionary<string, object>();
            VAlign = VerticalAlignment.Top;
        }
    }
}