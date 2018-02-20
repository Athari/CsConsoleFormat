using System;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public interface IInlineSequence
    {
        void AppendText([CanBeNull] string text);
        void PushColor(ConsoleColor color);
        void PushBackground(ConsoleColor background);
        void PopFormatting();
    }
}