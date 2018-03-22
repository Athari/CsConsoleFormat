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
            if (source is IEnumerable<ParsingError> errorList)
                return errorList.Select(ErrorInfo.FromError);
            else if (source is ParsingError error)
                return Enumerable.Repeat(ErrorInfo.FromError(error), 1);
            else {
                var stateProp = source?.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttributes<ParserStateAttribute>().FirstOrDefault() != null);
                if (stateProp == null || !stateProp.PropertyType.Is<IParserState>())
                    throw new ArgumentException("source must be options class with ParserStateAttribute on IParserState property.");
                var state = (IParserState)stateProp.GetValue(source, null);
                return state?.Errors.Select(ErrorInfo.FromError) ?? Enumerable.Empty<ErrorInfo>();
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