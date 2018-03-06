using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat
{
    public interface ILineCharRenderer
    {
        [Pure]
        char GetChar(LineChar lineChar);
    }
}