using System;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    public class Span : Element
    {
        public string Text { get; set; }
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? BgColor { get; set; }

        public Span (string text)
        {
            Text = text;
        }

        public Span () : this(null)
        {}

        public override string ToString ()
        {
            return base.ToString() + " Text=\"{0}\"".Fmt(Text);
        }
    }
}