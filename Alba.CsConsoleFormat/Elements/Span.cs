using System;
using System.Linq;

namespace Alba.CsConsoleFormat
{
    public class Span : InlineElement
    {
        public string Text { get; set; }

        public Span(string text)
        {
            Text = text;
        }

        public Span() : this(null)
        {}

        public override string GeneratedText =>
            Text ?? string.Concat(VisualChildren.Cast<InlineElement>().Select(c => c.GeneratedText));

        public override void GenerateSequence(IInlineSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (Color != null)
                sequence.PushColor(Color.Value);
            if (Background != null)
                sequence.PushBackground(Background.Value);

            if (Text != null) {
                sequence.AppendText(Text);
            }
            else {
                foreach (InlineElement child in VisualChildren.Cast<InlineElement>())
                    child.GenerateSequence(sequence);
            }

            if (Background != null)
                sequence.PopFormatting();
            if (Color != null)
                sequence.PopFormatting();
        }

        public override string ToString() => base.ToString() + $" Text=\"{Text}\"";
    }
}