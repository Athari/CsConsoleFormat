using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Testing.FluentAssertions
{
    [DebuggerNonUserCode]
    public static class StringAssertionsExts
    {
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "xUnit convention.")]
        public static AndConstraint<StringAssertions> BeLines([NotNull] this StringAssertions @this, [NotNull] params string[] lines)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));
            string actualText = @this.Subject;
            string expectedText = ConcatLines(lines);
            Execute.Assertion
                .ForCondition(actualText == expectedText)
                .FailWith($"Expected lines to be\n{FormatTextForError(expectedText)}\nbut found\n{FormatTextForError(actualText)}");
            return new AndConstraint<StringAssertions>(@this);
        }

        private static string ConcatLines([NotNull] string[] lines) => string.Join(Environment.NewLine, lines);

        private static string FormatTextForError([NotNull] string text) => string.Join(Environment.NewLine, text.Replace("\r", "").Split('\n'));
    }
}