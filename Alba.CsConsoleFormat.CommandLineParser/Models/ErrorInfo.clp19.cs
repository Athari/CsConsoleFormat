extern alias CommandLineParser_1_9;
using System.Runtime.CompilerServices;
using System.Text;
using CommandLineParser_1_9::CommandLine;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public partial class ErrorInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private ErrorInfo FromError19(object data)
        {
            var error = (ParsingError)data;
            Message = FormatParsingErrorMessage19(error);
            return this;
        }

        private static string FormatParsingErrorMessage19(ParsingError error)
        {
            var words = ClpUtils.SentenceBuilder19;
            var message = new StringBuilder();
            bool hasLong = error.BadOption.LongName?.Length > 0;
            if (error.BadOption.ShortName != null)
                message.Append($"-{error.BadOption.ShortName}{(hasLong ? "/" : "")}");
            if (hasLong)
                message.Append($"--{error.BadOption.LongName}");
            message.Append($" {(error.ViolatesRequired ? words.RequiredOptionMissingText : words.OptionWord)}");
            if (error.ViolatesFormat)
                message.Append($" {words.ViolatesFormatText}");
            if (error.ViolatesMutualExclusiveness) {
                if (error.ViolatesFormat || error.ViolatesRequired)
                    message.Append($" {words.AndWord}");
                message.Append($" {words.ViolatesMutualExclusivenessText}");
            }
            message.Append('.');
            return message.ToString();
        }
    }
}