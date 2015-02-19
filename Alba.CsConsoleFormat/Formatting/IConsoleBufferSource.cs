namespace Alba.CsConsoleFormat
{
    public interface IConsoleBufferSource
    {
        int Width { get; }
        int Height { get; }

        ConsoleChar[] GetLine (int y);
        ConsoleChar? GetChar (int x, int y);
        char GetLineChar (int x, int y);
        char SafeChar (char chr);
    }
}