using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat
{
    public class Span : InlineElement
    {
        public string Text { get; set; }

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