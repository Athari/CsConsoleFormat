using System.Diagnostics.CodeAnalysis;

namespace Alba.CsConsoleFormat
{
    public interface ILineCharRenderer
    {
        [SuppressMessage ("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray", Justification = "Each argument has specific purpose.")]
        char GetChar (LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom);
    }
}