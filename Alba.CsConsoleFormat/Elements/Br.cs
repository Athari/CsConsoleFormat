using System;

namespace Alba.CsConsoleFormat
{
    //[TrimSurroundingWhitespace]
    public class Br : InlineElement
    {
        protected override bool CanHaveChildren => false;

        public override string GeneratedText => "\n";

        public override void GenerateSequence (IInlineSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            sequence.AppendText("\n");
        }
    }
}