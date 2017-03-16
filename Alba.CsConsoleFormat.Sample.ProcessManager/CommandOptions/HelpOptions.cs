using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class HelpOptions
    {
        [ValueOption(0)]
        public string Verb { get; set; }

        [Option("all", HelpText = "Display help for all verbs.")]
        public bool All { get; set; }
    }
}