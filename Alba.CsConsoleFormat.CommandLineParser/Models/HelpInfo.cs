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
        [CanBeNull]
        private ICollection<Type> _optionsSource = new List<Type>();
        [CanBeNull]
        private object _errorsSource;

        /// <summary>
        /// Option types. Possible values:
        /// <list type="table">
        ///   <listheader><term>Version</term><description>Source</description></listheader>
        ///   <item><term>Version 1.9</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options type <c>typeof(MyOptions)</c></description></item>
        ///       <item><description>Root options type with verbs <c>typeof(MyRootVerbOptions)</c></description></item></list></description></item>
        ///   <item><term>Version 2.2</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options type <c>typeof(MyOptions)</c></description></item>
        ///       <item><description>List of verb options types <c>[ typeof(MyVerbFooOptions), typeof(MyVerbBarOptions) ]</c></description></item></list></description></item></list>
        /// </summary>
        [CanBeNull]
        public ICollection<Type> OptionsSource
        {
            get => _optionsSource;
            set
            {
                if (ReferenceEquals(_optionsSource, value))
                    return;
                _optionsSource = value;
                _options = null;
                _examples = null;
            }
        }

        /// <summary>
        /// Source of errors. Posible values:
        /// <list type="table">
        ///   <listheader><term>Version</term><description>Source</description></listheader>
        ///   <item><term>Version 1.9</term><description>
        ///     <list type="bullet">
        ///       <item><description>Options object with <c>ParserStateAttribute</c> on <c>IParserState</c> property.</description></item>
        ///       <item><description>Error list <c>IEnumerable&lt;ParsingError&gt;</c></description></item>
        ///       <item><description>Error <c>ParsingError</c></description></item></list></description></item>
        ///   <item><term>Version 2.2</term><description>
        ///     <list type="bullet">
        ///       <item><description>Parser result <c>typeof(MyRootVerbOptions)</c></description></item>
        ///       <item><description>Error list <c>IEnumerable&lt;Error&gt;</c></description></item>
        ///       <item><description>Error <c>Error</c></description></item></list></description></item>
        ///   <item><term>Any</term><description>
        ///     <list type="bullet">
        ///       <item><description>Error list <c>IEnumerable&lt;ErrorInfo&gt;</c></description></item>
        ///       <item><description>Error <c>ErrorInfo</c></description></item></list></description></item></list>
        /// </summary>
        [CanBeNull]
        public object ErrorsSource
        {
            get => _errorsSource;
            set
            {
                if (ReferenceEquals(_errorsSource, value))
                    return;
                _errorsSource = value;
                _errors = null;
            }
        }

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
                    var options = ClpUtils.GetOptionsFromOptionsRoots(OptionsSource.ToList()).ToList();
                    var optionKind = options.Any(o => o.OptionKind == OptionKind.Verb) ? OptionKind.Verb : OptionKind.Single;
                    if (optionKind != OptionKind.Verb) {
                        options = options
                            .OrderBy(o => o.IsPositional ? o.Index : int.MaxValue)
                            .ThenBy(o => o.Name)
                            .ToList();
                    }
                    _options = options;
                    if (ClpUtils.Features.Has(ClpFeatures.BuiltInHelpVersion)) {
                        _options.Add(OptionInfo.BuiltInHelp(optionKind));
                        _options.Add(OptionInfo.BuiltInVersion(optionKind));
                    }
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

        public bool ChooseVerb(string verbName)
        {
            var verb = Options.FirstOrDefault(o => string.Equals(o.Name, verbName, StringComparison.OrdinalIgnoreCase));
            if (verb == null)
                return false;
            _options = new List<OptionInfo>(1) { verb };
            return true;
        }

        internal HelpInfo With(ICollection<Type> optionsSource = null, object errorsSource = null)
        {
            var copy = (HelpInfo)MemberwiseClone();
            if (optionsSource != null)
                copy.OptionsSource = optionsSource;
            if (errorsSource != null)
                copy.ErrorsSource = errorsSource;
            return copy;
        }
    }
}