using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [Verb("list", HelpText = "List running processes.\nSYNTAX: ProcessManager list [options]")]
    internal sealed class ListOptions
    {
        [Option('n', "name", HelpText = "If specified, filter by the specified process name.")]
        public string ProcessName { get; set; }

        [Option('m', "machine", Default = ".", HelpText = "If specified, get processes of the specified machine, otherwise local processes.")]
        public string MachineName { get; set; }

        [Option("withtitle", HelpText = "If specified, display only processes with non-empty main window title.")]
        public bool WithTitle { get; set; }

        [Usage(ApplicationAlias = "ProcessManager")]
        public static IEnumerable<Example> Examples => new[] {
            new Example(
                "List all processes on local machine",
                new ListOptions()),
            new Example(
                "List processes having main window with title",
                new ListOptions { WithTitle = true }),
            new Example(
                "List remote Visual Studio processes",
                new ListOptions { ProcessName = "devenv", MachineName = "alice" }),
        };
    }
}