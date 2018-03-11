using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat
{
    public class Document : Div
    {
        public Dictionary<string, object> Resources { get; } = new Dictionary<string, object>();
        public ILineCharRenderer LineCharRenderer { get; set; } = null;

        public Document()
        { }

        public Document(params object[] children) : base(children)
        { }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Force document to occupy whole availableSize.Width (usually Console.BufferWidth).
            Size size = base.MeasureOverride(availableSize);
            return new Size(Math.Max(size.Width, availableSize.Width), size.Height);
        }
    }
}