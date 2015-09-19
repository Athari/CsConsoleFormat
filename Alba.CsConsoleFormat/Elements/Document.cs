using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class Document : Div
    {
        public Dictionary<string, object> Resources { get; } = new Dictionary<string, object>();

        public Document ()
        {
            VAlign = VerticalAlignment.Top;
        }
    }
}