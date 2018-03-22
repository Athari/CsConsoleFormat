﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using Alba.CsConsoleFormat.CommandLineParser;
using Alba.CsConsoleFormat.Sample.ProcessManager.CommandOptions;
using CommandLine;

[assembly: AssemblyLicense("My license is cool.", "It's called WTFPL*.")]
[assembly: AssemblyUsage("My usage is cool.", "Type whatever you want.")]

// ReSharper disable AnnotateNotNullParameter
// ReSharper disable MemberCanBeMadeStatic.Local
namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal sealed class Program
    {
        private static void Main(string[] args) => new Program().Run(args);

        private void Run(string[] args)
        {
            try {
                Console.OutputEncoding = Encoding.UTF8;
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
                ConsoleRenderer.RenderDocument(View.Error(ex.Message, ex.ToString()));
            }
            finally {
                if (Debugger.IsAttached) {
                    ConsoleRenderer.RenderDocument(new HelpView {
                        Header = "Header",
                        Footer = "Footer",
                    }.Help(HelpParts.AssemblyMeta));
                    ConsoleRenderer.RenderDocument(View.Info("Press any key..."));
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
            var options = new HelpInfo{ OptionsSource = {typeof(RootOptions)} }.Options;
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