using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.CommandLineParser;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class HelpOptions
    {
        [ValueOption(1), OptionMeta(HelpText = "Command to display help for.")]
        public string Verb { get; set; }

        [Option("all", HelpText = "Display information on all commands.")]
        public bool All { get; set; }
    }
}