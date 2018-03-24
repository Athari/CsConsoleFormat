using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using Alba.CsConsoleFormat.CommandLineParser;
using Alba.CsConsoleFormat.Fluent;
using Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions;
using CommandLine;

[assembly: AssemblyLicense(
    "Sample project is released under Apache 2.0 license.")]
[assembly: AssemblyUsage(
    "Simple utility for listing and starting processes. Demonstrates usage of CsConsoleFormat.",
    "SYNTAX: ProcessManager command [options]")]

// ReSharper disable AnnotateNotNullParameter
// ReSharper disable MemberCanBeMadeStatic.Local
namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal sealed class Program
    {
        private readonly HelpView HelpView = new HelpView {
            Info = { OptionsSource = { typeof(RootOptions) } },
            OptionNameMaxLength = 16,
        };

        private static void Main(string[] args) => new Program().Run(args);

        private void Run(string[] args)
        {
            if (Debugger.IsAttached) {
                try {
                    Console.BufferWidth = Console.WindowWidth = 80;
                }
                catch { }
            }
            try {
                Console.OutputEncoding = Encoding.UTF8;
                var options = new RootOptions();
                var parser = new Parser(s => s.HelpWriter = null);
                if (!parser.ParseArguments(args, options, (s, v) => { }))
                    InvokeError(options);
                else if (options.List != null)
                    InvokeList(options.List);
                else if (options.Start != null)
                    InvokeStart(options.Start);
                else
                    InvokeHelp(options.Help);
            }
            catch (Exception ex) {
                HelpView.Message(ErrorInfo.Exception(ex)).Render();
            }
            finally {
                if (Debugger.IsAttached) {
                    View.PressEnter().Render();
                    Console.ReadLine();
                }
            }
        }

        private void InvokeError(RootOptions error)
        {
            HelpView.HelpNotParsed(error).Render();
        }

        private void InvokeList(ListOptions list)
        {
            IEnumerable<Process> processes = list.ProcessName != null
                ? Process.GetProcessesByName(list.ProcessName, list.MachineName)
                : Process.GetProcesses(list.MachineName);
            if (list.WithTitle)
                processes = processes.Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            processes = processes.OrderByDescending(p => p.StartTime).Take(list.Limit);
            View.ProcessList(processes).Render();
        }

        private void InvokeStart(StartOptions start)
        {
            var password = new SecureString();
            if (start.Password != null) {
                foreach (char c in start.Password)
                    password.AppendChar(c);
            }
            Process.Start(start.FileName, start.Arguments, start.UserName, password, start.Domain);
            HelpView.Message(ErrorInfo.Info($"Started {start.FileName}.")).Render();
        }

        private void InvokeHelp(HelpOptions help)
        {
            HelpView.HelpVerb(help.Verb, help.All).Render();
        }
    }
}