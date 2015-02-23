using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public interface ILineCharRenderer
    {
        [SuppressMessage ("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        char GetChar (LineChar chr, LineChar chrLeft, LineChar chrTop, LineChar chrRight, LineChar chrBottom);
    }
}