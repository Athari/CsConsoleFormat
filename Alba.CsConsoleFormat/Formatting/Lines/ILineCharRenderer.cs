using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Alba.CsConsoleFormat
{
    public interface ILineCharRenderer
    {
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray", Justification = "Each argument has specific purpose.")]
        char GetChar(LineChar charCenter, LineChar charLeft, LineChar charTop, LineChar charRight, LineChar charBottom);
    }
}