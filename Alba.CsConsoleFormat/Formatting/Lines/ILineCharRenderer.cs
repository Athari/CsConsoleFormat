namespace Alba.CsConsoleFormat
{
    public interface ILineCharRenderer
    {
        char GetChar (LineChar chr, LineChar chrLeft, LineChar chrTop, LineChar chrRight, LineChar chrBottom);
    }
}