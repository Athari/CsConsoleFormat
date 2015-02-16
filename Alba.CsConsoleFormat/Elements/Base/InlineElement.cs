namespace Alba.CsConsoleFormat
{
    public abstract class InlineElement : Element
    {
        public abstract string GeneratedText { get; }

        public abstract void GenerateSequence (IInlineSequence sequence);
    }
}