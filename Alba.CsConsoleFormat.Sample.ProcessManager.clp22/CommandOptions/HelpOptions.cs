using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [Verb("help-all", HelpText = "Display information on all commands.")]
    internal sealed class HelpOptions
    {
        [Value(0, MetaName = "verb", HelpText = "Command to display help for.", Hidden = true)]
        public string Verb { get; set; }

        [Option("all", HelpText = "Display help for all commands.", Hidden = true)]
        public bool All { get; set; }
    }
}