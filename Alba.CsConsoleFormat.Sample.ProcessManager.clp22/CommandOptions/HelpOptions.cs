using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [Verb("hlp", HelpText = "Display help.")]
    internal sealed class HelpOptions
    {
        [Value(0, MetaName = "verb")]
        public string Verb { get; set; }

        [Option("all", HelpText = "Display help for all verbs.")]
        public bool All { get; set; }
    }
}