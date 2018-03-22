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
using CommandLine.Text;

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
        private static readonly Type[] OptionsTypes = { typeof(ListOptions), typeof(StartOptions), typeof(HelpOptions) };
        private static readonly HelpView HelpView = new HelpView { Info = { OptionsSource = OptionsTypes } };

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
                var result = new Parser(s => s.HelpWriter = null).ParseArguments(args, OptionsTypes);
                result
                    .WithNotParsed(InvokeError)
                    .WithParsed<ListOptions>(InvokeList)
                    .WithParsed<StartOptions>(InvokeStart)
                    .WithParsed<HelpOptions>(InvokeHelp);
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

        private void InvokeError(IEnumerable<Error> errors)
        {
            HelpView.HelpNotParsed(errors).Render();
        }

        private void InvokeList(ListOptions list)
        {
            IEnumerable<Process> processes = list.ProcessName != null
                ? Process.GetProcessesByName(list.ProcessName, list.MachineName)
                : Process.GetProcesses(list.MachineName);
            if (list.WithTitle)
                processes = processes.Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            processes = processes.OrderByDescending(p => p.StartTime);
            View.ProcessList(processes).Render();
        }

        private void InvokeStart(StartOptions start)
        {
            var password = new SecureString();
            if (start.Password != null) {
                foreach (char c in start.Password)
                    password.AppendChar(c);
            }

            var process = Process.Start(start.FileName, start.Arguments, start.UserName, password, start.Domain);
            HelpView.Message(ErrorInfo.Info($"Started {start.FileName}.")).Render();

            if (process == null)
                return;
            switch (start.Operation) {
                case StartOperation.WaitForExit:
                    process.WaitForExit();
                    break;
                case StartOperation.WaitForIdle:
                    process.WaitForInputIdle();
                    break;
                case StartOperation.Kill:
                    process.Kill();
                    break;
                case StartOperation.DisplayFullName:
                    HelpView.Message(ErrorInfo.Info(process.MainModule.FileName)).Render();
                    break;
            }
        }

        private void InvokeHelp(HelpOptions help)
        {
            HelpView.Help(HelpParts.All).Render();
        }
    }
}