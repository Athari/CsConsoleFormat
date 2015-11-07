using System;
using System.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Alba.CsConsoleFormat.Testing.FluentAssertions
{
    [DebuggerNonUserCode]
    public static class StringAssertionsExts
    {
        public static AndConstraint<StringAssertions> BeLines (this StringAssertions @this, params string[] lines)
        {
            string actualText = @this.Subject;
            string expectedText = ConcatLines(lines);
            Execute.Assertion
                .ForCondition(actualText == expectedText)
                .FailWith($"Expected lines to be\n{FormatTextForError(expectedText)}\nbut found\n{FormatTextForError(actualText)}");
            return new AndConstraint<StringAssertions>(@this);
        }

        private static string ConcatLines (string[] lines) => string.Join(Environment.NewLine, lines);

        private static string FormatTextForError (string text) => string.Join(Environment.NewLine, text.Replace("\r", "").Split('\n'));
    }
}