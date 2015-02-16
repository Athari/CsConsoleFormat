using System;

namespace Alba.CsConsoleFormat
{
    public interface IInlineSequence
    {
        void AppendText (string str);
        void PushColor (ConsoleColor color);
        void PushBgColor (ConsoleColor bgColor);
        void PopFormatting ();
    }
}