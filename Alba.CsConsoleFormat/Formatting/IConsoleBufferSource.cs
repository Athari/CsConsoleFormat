using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat
{
    public interface IConsoleBufferSource
    {
        int Width { get; }
        int Height { get; }

        [Pure]
        ConsoleChar[] GetLine(int y);

        [Pure]
        ConsoleChar? GetChar(int x, int y);

        [Pure]
        char GetLineChar(int x, int y);
    }
}