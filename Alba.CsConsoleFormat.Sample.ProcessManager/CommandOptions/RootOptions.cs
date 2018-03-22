using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class RootOptions
    {
        [VerbOption("list", HelpText = "List running processes.\nSYNTAX: ProcessManager list [options]")]
        public ListOptions List { get; set; }

        [VerbOption("start", HelpText = "Start a new process.\nSYNTAX: ProcessManager start filename [options]")]
        public StartOptions Start { get; set; }

        [VerbOption("help", HelpText = "Display information on a specific command.")]
        public HelpOptions Help { get; set; }
    }
}