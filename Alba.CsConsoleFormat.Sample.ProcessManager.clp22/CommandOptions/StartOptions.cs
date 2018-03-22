using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [Verb("start", HelpText = "Start a new process.")]
    internal sealed class StartOptions
    {
        [Value(0, MetaName = "filename")]
        public string FileName { get; set; }

        [Option('a', "args", HelpText = "Command-line arguments to pass when starting the process.")]
        public string Arguments { get; set; }

        [Option('d', "domain", HelpText = "The domain to use when starting the process.")]
        public string Domain { get; set; }

        [Option('u', "user", HelpText = "The user name to use when starting the process.")]
        public string UserName { get; set; }

        [Option('p', "password", HelpText = "The password to use when starting the process.")]
        public string Password { get; set; }

        [Usage(ApplicationAlias = "ProcessManager")]
        public static IEnumerable<Example> Examples => new[] {
            new Example(
                "Start notepad locally",
                new StartOptions { FileName = "notepad" }),
            new Example(
                "Start notepad on remote computer",
                new[] {
                    new UnParserSettings(),
                    new UnParserSettings { UseEqualToken = true },
                    new UnParserSettings { PreferShortName = true },
                },
                new StartOptions { FileName = "notepad", Arguments = "C:\\config.sys", Domain = "localhost", UserName = "somebody" }),
        };
    }
}