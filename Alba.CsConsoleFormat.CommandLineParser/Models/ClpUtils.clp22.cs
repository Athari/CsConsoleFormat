extern alias CommandLineParser_2_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineParser_2_2::CommandLine;
using CommandLineParser_2_2::CommandLine.Text;
#if NET_40 || NET_STANDARD_15
using Alba.CsConsoleFormat.CommandLineParser.Framework;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser
{
    internal partial class ClpUtils
    {
        private static readonly string VerbAttributeTypeName22 = $"CommandLine.{nameof(VerbAttribute)}";

        public static SentenceBuilder SentenceBuilder22
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get => SentenceBuilder.Create();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<ExampleInfo> GetExamplesFromOptions22(IEnumerable<PropertyInfo> props)
        {
            UsageAttribute usageAttr = null;
            var examplesProp = props.FirstOrDefault(p => (usageAttr = p.GetCustomAttributes<UsageAttribute>().FirstOrDefault()) != null);
            if (examplesProp == null)
                return Enumerable.Empty<ExampleInfo>();
            if (!examplesProp.PropertyType.Is<IEnumerable<Example>>() || !examplesProp.GetGetMethod().IsStatic)
                throw new InvalidOperationException("Property decorated with UsageAttribute must be static and of type IEnumerable<Example>.");
            var examples = (IEnumerable<Example>)examplesProp.GetValue(null, null);
            return examples.Select(e => ExampleInfo.FromExample(e, Nullable(usageAttr.ApplicationAlias)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<ErrorInfo> GetErrorsFromParserResultOrErrorList22(object source)
        {
            if (source is IEnumerable<Error> errorList)
                return errorList.Select(ErrorInfo.FromError);
            else if (source is Error error)
                return Enumerable.Repeat(ErrorInfo.FromError(error), 1);
            else {
                Type type = source?.GetType();
                if (type?.Name == $"{nameof(Parsed<object>)}`1")
                    return new List<ErrorInfo>();
                if (type?.Name != $"{nameof(NotParsed<object>)}`1")
                    throw new ArgumentException("source must be ParserResult<T>.", nameof(source));
                var errorsProp = type.GetProperty(nameof(NotParsed<object>.Errors));
                if (errorsProp == null)
                    throw new ArgumentException("source must be ParserResult<T>.", nameof(source));
                var errors = (IEnumerable<Error>)errorsProp.GetValue(source, null);
                return errors.Select(ErrorInfo.FromError);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetAssemblyUsageText22(Assembly assembly) =>
            assembly.GetCustomAttributes<AssemblyUsageAttribute>().FirstOrDefault()?.Value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetAssemblyLicenseText22(Assembly assembly) =>
            assembly.GetCustomAttributes<AssemblyLicenseAttribute>().FirstOrDefault()?.Value;
    }
}