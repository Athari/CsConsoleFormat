using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions;
using CommandLine;

namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal class Program
    {
        private readonly View _view = new View();

        private static void Main(string[] args) => new Program().Run(args);

        private void Run(string[] args)
        {
            try {
                if (args.Length == 0) {
                    InvokeHelp(new HelpOptions());
                    return;
                }

                var options = new RootOptions();
                if (!Parser.Default.ParseArguments(args, options, (s, o) => { }))
                    Environment.Exit(Parser.DefaultExitCodeFail);

                if (options.List != null)
                    InvokeList(options.List);
                else if (options.Start != null)
                    InvokeStart(options.Start);
                else
                    InvokeHelp(options.Help);
            }
            catch (Exception ex) {
                ConsoleRenderer.RenderDocument(_view.Error(ex.Message, ex.ToString()));
            }
            finally {
                if (Debugger.IsAttached) {
                    ConsoleRenderer.RenderDocument(_view.Info("Press any key..."));
                    Console.ReadLine();
                }
            }
        }

        private void InvokeList(ListOptions list)
        {
            IEnumerable<Process> processes = list.ProcessName != null
                ? Process.GetProcessesByName(list.ProcessName, list.MachineName)
                : Process.GetProcesses(list.MachineName);
            if (list.WithTitle)
                processes = processes.Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            processes = processes.OrderByDescending(p => p.StartTime);
            ConsoleRenderer.RenderDocument(_view.ProcessList(processes));
        }

        private void InvokeStart(StartOptions start)
        {
            var password = new SecureString();
            if (start.Password != null) {
                foreach (char c in start.Password)
                    password.AppendChar(c);
            }
            ConsoleRenderer.RenderDocument(_view.Info($"Starting {start.FileName}..."));
            Process.Start(start.FileName, start.Arguments, start.UserName, password, start.Domain);
        }

        private void InvokeHelp(HelpOptions help)
        {
            string instruction = "Syntax: ProcessManager.exe verb [options]\n\nAvailable verbs:";
            if (help.All) {
                var allOptions = CommandLineOptions.GetAllOptions(typeof(RootOptions));
                ConsoleRenderer.RenderDocument(_view.HelpAllOptionsList(allOptions, instruction));
                return;
            }

            Type optionsType = typeof(RootOptions);
            if (help.Verb != null) {
                instruction = $"Syntax: ProcessManager.exe {help.Verb} [options]\n\nAvailable {help.Verb} options:";
                optionsType = CommandLineOptions.GetVerbTypeByName(optionsType, help.Verb);
            }

            if (optionsType == null) {
                ConsoleRenderer.RenderDocument(_view.Error($"Verb {help.Verb} not supported."));
                return;
            }

            var options = CommandLineOptions.GetOptions(optionsType);
            ConsoleRenderer.RenderDocument(_view.HelpOptionsList(options, instruction));
        }
    }
}