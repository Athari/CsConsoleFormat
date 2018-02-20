using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class HelpOptions
    {
        [ValueOption(0)]
        public string Verb { get; set; }

        [Option("all", HelpText = "Display help for all verbs.")]
        public bool All { get; set; }
    }
}