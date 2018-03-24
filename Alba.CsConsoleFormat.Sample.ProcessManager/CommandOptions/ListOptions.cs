using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class ListOptions
    {
        [Option('n', "name", HelpText = "If specified, filter by the specified process name.")]
        public string ProcessName { get; set; }

        [Option('m', "machine", DefaultValue = ".", HelpText = "If specified, get processes of the specified machine, otherwise local processes.")]
        public string MachineName { get; set; }

        [Option("withtitle", HelpText = "If specified, display only processes with non-empty main window title.")]
        public bool WithTitle { get; set; }

        [Option('l', "limit", DefaultValue = 30, MetaValue = "INT", HelpText = "If specified, limit the number of listed processes with the specified number.")]
        public int Limit { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}