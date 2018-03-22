using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.CommandLineParser;
using CommandLine;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class StartOptions
    {
        [ValueOption(1), OptionMeta(Required = true, HelpText = "Executable file name.")]
        public string FileName { get; set; }

        [Option('a', "args", HelpText = "Command-line arguments to pass when starting the process.")]
        public string Arguments { get; set; }

        [Option('d', "domain", HelpText = "The domain to use when starting the process.")]
        public string Domain { get; set; }

        [Option('u', "user", HelpText = "The user name to use when starting the process.")]
        public string UserName { get; set; }

        [Option('p', "password", HelpText = "The password to use when starting the process.")]
        public string Password { get; set; }
    }
}