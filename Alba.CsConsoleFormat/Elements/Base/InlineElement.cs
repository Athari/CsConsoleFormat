using System.ComponentModel;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public abstract class InlineElement : Element
    {
        [NotNull]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract string GeneratedText { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract void GenerateSequence([NotNull] IInlineSequence sequence);
    }
}