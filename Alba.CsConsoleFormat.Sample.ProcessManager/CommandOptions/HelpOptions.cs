using CommandLine;

namespace Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions
{
    internal class HelpOptions
    {
        [ValueOption (0)]
        public string Verb { get; set; }
    }
}