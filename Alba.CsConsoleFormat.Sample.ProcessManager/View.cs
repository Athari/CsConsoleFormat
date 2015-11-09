using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal class View
    {
        public Document Error (string message, string title = null)
        {
            return new Document { Background = Black }
                .AddChildren(
                    new Div { Color = Red }.AddChildren("Error:"),
                    title != null
                        ? new Div { Color = Red }.AddChildren(title)
                        : null,
                    new Div { Color = White }.AddChildren(message)
                );
        }

        public Document Info (string message)
        {
            return new Document { Background = Black, Color = Gray }
                .AddChildren(message);
        }

        public Document ProcessList (IEnumerable<Process> processes)
        {
            var strokeHeader = new LineThickness(LineWidth.None, LineWidth.Single);
            var marginRight = new Thickness(0, 0, 1, 0);

            return new Document { Background = Black, Color = White }
                .AddChildren(
                    new Grid { Stroke = LineThickness.None }
                        .AddColumns(
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Auto, MaxWidth = 20 },
                            new Column { Width = GridLength.Star(1) },
                            new Column { Width = GridLength.Auto }
                        )
                        .AddChildren(
                            new Cell { Stroke = strokeHeader }
                                .AddChildren("Id"),
                            new Cell { Stroke = strokeHeader }
                                .AddChildren("Name"),
                            new Cell { Stroke = strokeHeader }
                                .AddChildren("Main Window Title"),
                            new Cell { Stroke = strokeHeader }
                                .AddChildren("Private Memory"),
                            processes.Select(p => new[] {
                                new Div { Margin = marginRight }
                                    .AddChildren(p.Id),
                                new Div { Margin = marginRight, Color = Yellow, TextWrap = TextWrapping.CharWrap }
                                    .AddChildren(p.ProcessName),
                                new Div { Margin = marginRight, MaxHeight = 1 }
                                    .AddChildren(p.MainWindowTitle),
                                new Div { Align = HorizontalAlignment.Right }
                                    .AddChildren(p.PrivateMemorySize64.ToString("n0")),
                            })
                        )
                );
        }

        public Document HelpOptionsList (IEnumerable<BaseOptionAttribute> options, string instruction)
        {
            return new Document { Background = Black }
                .AddChildren(
                    new Div { Color = White }
                        .AddChildren(instruction),
                    "",
                    new Grid { Stroke = LineThickness.None }
                        .AddColumns(
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Star(1) }
                        )
                        .AddChildren(
                            options.Select(o => new[] {
                                new Div { Margin = new Thickness(1, 0, 2, 1), Color = Yellow }
                                    .AddChildren(GetOptionSyntax(o)),
                                new Div { Color = Gray }
                                    .AddChildren(o.HelpText),
                            })
                        )
                );
        }

        private object GetOptionSyntax (BaseOptionAttribute option)
        {
            if (option is VerbOptionAttribute)
                return option.LongName;
            else if (option.ShortName != null) {
                if (option.LongName != null)
                    return $"--{option.LongName}, -{option.ShortName}";
                else
                    return $"-{option.ShortName}";
            }
            else if (option.LongName != null)
                return $"--{option.LongName}";
            else
                return "";
        }
    }
}