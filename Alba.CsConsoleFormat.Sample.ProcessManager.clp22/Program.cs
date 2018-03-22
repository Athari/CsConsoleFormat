using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using Alba.CsConsoleFormat.CommandLineParser;
using Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions;
using CommandLine;
using CommandLine.Text;

[assembly: AssemblyLicense("Sample project is released under Apache 2.0 license.")]
[assembly: AssemblyUsage("Simple utility for listing and starting processes.", "Demonstrates usage of CsConsoleFormat.")]

// ReSharper disable AnnotateNotNullParameter
// ReSharper disable MemberCanBeMadeStatic.Local
namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal sealed class Program
    {
        private static readonly Type[] OptionsTypes = { typeof(ListOptions), typeof(StartOptions), typeof(HelpOptions) };

        private static void Main(string[] args) => new Program().Run(args);

        private HelpView HelpView = new HelpView { Info = { OptionsSource = OptionsTypes } };

        private void Run(string[] args)
        {
            try {
                Console.OutputEncoding = Encoding.UTF8;
                var result = new Parser(s => s.HelpWriter = null).ParseArguments(args, OptionsTypes);
                result
                    .WithNotParsed(InvokeError)
                    .WithParsed<ListOptions>(InvokeList)
                    .WithParsed<StartOptions>(InvokeStart)
                    .WithParsed<HelpOptions>(InvokeHelp);
            }
            catch (Exception ex) {
                ConsoleRenderer.RenderDocument(HelpView.HelpError(ex));
            }
            finally {
                if (Debugger.IsAttached) {
                    ConsoleRenderer.RenderDocument(View.Info("Press any key..."));
                    Console.ReadLine();
                }
            }
        }

        private void InvokeError(IEnumerable<Error> errors)
        {
            ConsoleRenderer.RenderDocument(HelpView.HelpError(errors));
        }

        private void InvokeList(ListOptions list)
        {
            IEnumerable<Process> processes = list.ProcessName != null
                ? Process.GetProcessesByName(list.ProcessName, list.MachineName)
                : Process.GetProcesses(list.MachineName);
            if (list.WithTitle)
                processes = processes.Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            processes = processes
                .Select(process => {
                    try {
                        return new { process, time = process.StartTime };
                    }
                    catch {
                        return new { process = (Process)null, time = DateTime.MinValue };
                    }
                })
                .Where(p => p.time != DateTime.MinValue)
                .Select(p => p.process)
                .OrderByDescending(p => p.StartTime);
            ConsoleRenderer.RenderDocument(View.ProcessList(processes));
        }

        private void InvokeStart(StartOptions start)
        {
            var password = new SecureString();
            if (start.Password != null) {
                foreach (char c in start.Password)
                    password.AppendChar(c);
            }
            ConsoleRenderer.RenderDocument(View.Info($"Starting {start.FileName}..."));
            Process.Start(start.FileName, start.Arguments, start.UserName, password, start.Domain);
        }

        private void InvokeHelp(HelpOptions help)
        {
            var options = new HelpInfo { OptionsSource = OptionsTypes }.Options;
            string instruction = "Syntax: ProcessManager.exe verb [options]\n\nAvailable verbs:";
            if (help.All) {
                ConsoleRenderer.RenderDocument(View.HelpAllOptionsList(options, instruction));
                return;
            }

            if (help.Verb != null) {
                instruction = $"Syntax: ProcessManager.exe {help.Verb} [options]\n\nAvailable {help.Verb} options:";
                options = options.FirstOrDefault(o => o.Name == help.Verb)?.SubOptions;
                if (options == null) {
                    ConsoleRenderer.RenderDocument(View.Error($"Verb {help.Verb} not supported."));
                    return;
                }
            }

            ConsoleRenderer.RenderDocument(View.HelpOptionsList(options, instruction));
        }
    }
}