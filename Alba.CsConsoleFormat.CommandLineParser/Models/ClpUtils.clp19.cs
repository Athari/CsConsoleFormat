extern alias CommandLineParser_1_9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineParser_1_9::CommandLine;
using CommandLineParser_1_9::CommandLine.Text;
#if NET_40 || NET_STANDARD_15
using Alba.CsConsoleFormat.CommandLineParser.Framework;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser
{
    internal partial class ClpUtils
    {
        private static readonly string VerbOptionAttributeTypeName19 = $"CommandLine.{nameof(VerbOptionAttribute)}";

        public static BaseSentenceBuilder SentenceBuilder19
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get => BaseSentenceBuilder.CreateBuiltIn();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<ErrorInfo> GetErrorsFromParserStateOrErrorList19(object source)
        {
            switch (source) {
                case null:
                    return Enumerable.Empty<ErrorInfo>();
                case IEnumerable<ParsingError> errorList:
                    return errorList.Select(ErrorInfo.FromError);
                case ParsingError error:
                    return Enumerable.Repeat(ErrorInfo.FromError(error), 1);
                default:
                    var props = source.GetType().GetProperties();
                    var errors = props
                        .Where(p => p.GetCustomAttributes<VerbOptionAttribute>().FirstOrDefault() != null)
                        .Select(p => p.GetValue(source, null))
                        .SelectMany(GetErrorsFromParserStateOrErrorList19);
                    var stateProp = props.FirstOrDefault(p => p.GetCustomAttributes<ParserStateAttribute>().FirstOrDefault() != null);
                    if (stateProp?.PropertyType.Is<IParserState>() == true) {
                        var state = (IParserState)stateProp.GetValue(source, null);
                        if (state != null)
                            errors = errors.Concat(state.Errors.Select(ErrorInfo.FromError));
                    }
                    return errors;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetAssemblyUsageText19(Assembly assembly) =>
            assembly.GetCustomAttributes<AssemblyUsageAttribute>().FirstOrDefault()?.Value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetAssemblyLicenseText19(Assembly assembly) =>
            assembly.GetCustomAttributes<AssemblyLicenseAttribute>().FirstOrDefault()?.Value;
    }
}