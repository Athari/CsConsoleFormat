using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class Document : Div
    {
        public Dictionary<string, object> Resources { get; } = new Dictionary<string, object>();
        public ILineCharRenderer LineCharRenderer { get; set; } = null;

        public Document()
        {
            VerticalAlign = VerticalAlign.Top;
        }
    }
}