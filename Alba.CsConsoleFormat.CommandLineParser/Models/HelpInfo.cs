using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public sealed class HelpInfo
    {
        private List<OptionInfo> _options;
        private List<ErrorInfo> _errors;
        private ILookup<OptionInfo, ExampleInfo> _examples;

        /// <summary>
        /// Option types.
        /// <list type="table">
        ///   <listheader><term>Version</term><description>Source</description></listheader>
        ///   <item><term>Version 1.9</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options type <c>typeof(MyOptions)</c> -or-</description></item>
        ///       <item><description>Root options type with verbs <c>typeof(MyRootVerbOptions)</c></description></item></list></description></item>
        ///   <item><term>Version 2.2</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options type <c>typeof(MyOptions)</c> -or-</description></item>
        ///       <item><description>List of verb options types <c>[ typeof(MyVerbFooOptions), typeof(MyVerbBarOptions) ]</c></description></item></list></description></item></list>
        /// </summary>
        [CanBeNull]
        public ICollection<Type> OptionsSource { get; set; } = new List<Type>();

        /// <summary>
        /// Source of errors.
        /// <list type="table">
        ///   <listheader><term>Version</term><description>Source</description></listheader>
        ///   <item><term>Version 1.9</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options object with <c>ParserStateAttribute</c> on <c>IParserState</c> property. -or-</description></item>
        ///       <item><description>Error list <c>IEnumerable&lt;ParsingError&gt;</c></description></item></list></description></item>
        ///   <item><term>Version 2.2</term><description>
        ///     <list type="bullet">
        ///       <item><description>Parser result <c>typeof(MyRootVerbOptions)</c> -or-</description></item>
        ///       <item><description>Error list <c>IEnumerable&lt;Error&gt;</c></description></item></list></description></item></list>
        /// </summary>
        [CanBeNull]
        public object ErrorsSource { get; set; }

        public HelpParts Parts => 0
          | (OptionsSource != null ? HelpParts.Options : 0)
          | (OptionsSource != null && ClpUtils.Features.Has(ClpFeatures.Examples) ? HelpParts.Examples : 0)
          | (ErrorsSource != null ? HelpParts.Errors : 0);

        public IList<OptionInfo> Options
        {
            get
            {
                if (_options == null) {
                    if (!(OptionsSource?.Count > 0))
                        throw new InvalidOperationException($"{nameof(OptionsSource)} not set or empty.");
                    _options = ClpUtils.GetOptionsFromOptionsRoots(OptionsSource.ToList())
                        .OrderBy(o => o.IsPositional ? o.Index : int.MaxValue)
                        .ThenBy(o => o.Name)
                        .ToList();
                }
                return new ReadOnlyCollection<OptionInfo>(_options);
            }
        }

        public IList<ErrorInfo> Errors
        {
            get
            {
                if (_errors == null) {
                    if (ErrorsSource == null)
                        throw new InvalidOperationException($"{nameof(ErrorsSource)} not set.");
                    _errors = ClpUtils.GetErrorsFromParserResultOrParserStateOrErrorList(ErrorsSource);
                }
                return new ReadOnlyCollection<ErrorInfo>(_errors);
            }
        }

        public ILookup<OptionInfo, ExampleInfo> Examples
        {
            get
            {
                if (_examples == null) {
                    if (!(OptionsSource?.Count > 0))
                        throw new InvalidOperationException($"{nameof(OptionsSource)} not set or empty.");
                    _examples = ClpUtils.GetExamplesFromOptionsRoots(OptionsSource.ToList(), Options);
                }
                return _examples;
            }
        }

        internal HelpInfo Clone() => (HelpInfo)MemberwiseClone();
    }
}