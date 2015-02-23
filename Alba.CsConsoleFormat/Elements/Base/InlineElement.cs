using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    public abstract class InlineElement : Element
    {
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public abstract string GeneratedText { get; }

        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public abstract void GenerateSequence (IInlineSequence sequence);
    }
}