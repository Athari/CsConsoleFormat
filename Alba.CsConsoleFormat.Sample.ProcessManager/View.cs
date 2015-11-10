using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal class View
    {
        public Document Error (string message, string extra = null)
        {
            return new Document { Background = Black }
                .AddChildren(
                    new Div { Color = Red }.AddChildren("Error"),
                    message != null
                        ? new Div { Color = White }.AddChildren(message)
                        : null,
                    extra != null
                        ? new object[] {
                            "",
                            new Div { Color = Gray }.AddChildren(extra)
                        }
                        : null
                );
        }

        public Document Info (string message)
        {
            return new Document { Background = Black, Color = Gray }
                .AddChildren(message);
        }

        public Document ProcessList (IEnumerable<Process> processes)
        {
            var strokeHeader = new LineThickness(LineWidth.None, LineWidth.Wide);
            var strokeRight = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);
            var strokeBottom = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.None, LineWidth.Wide);

            return new Document { Background = Black, Color = Gray }
                .AddChildren(
                    new Grid { Stroke = strokeBottom, StrokeColor = DarkGray }
                        .AddColumns(
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Auto, MaxWidth = 20 },
                            new Column { Width = GridLength.Star(1) },
                            new Column { Width = GridLength.Auto }
                        )
                        .AddChildren(
                            new Cell { Stroke = strokeHeader, Color = White }
                                .AddChildren("Id"),
                            new Cell { Stroke = strokeHeader, Color = White }
                                .AddChildren("Name"),
                            new Cell { Stroke = strokeHeader, Color = White }
                                .AddChildren("Main Window Title"),
                            new Cell { Stroke = strokeHeader, Color = White }
                                .AddChildren("Private Memory"),
                            processes.Select(process => new[] {
                                new Cell { Stroke = strokeRight }
                                    .AddChildren(process.Id),
                                new Cell { Stroke = strokeRight, Color = Yellow, TextWrap = TextWrapping.NoWrap }
                                    .AddChildren(process.ProcessName),
                                new Cell { Stroke = strokeRight, Color = White, TextWrap = TextWrapping.NoWrap }
                                    .AddChildren(process.MainWindowTitle),
                                new Cell { Stroke = LineThickness.None, Align = HorizontalAlignment.Right }
                                    .AddChildren(process.PrivateMemorySize64.ToString("n0")),
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
                            GridLength.Auto,
                            GridLength.Star(1)
                        )
                        .AddChildren(
                            options.Select(option => new[] {
                                new Div { Margin = new Thickness(1, 0, 1, 1), Color = Yellow }
                                    .AddChildren(GetOptionSyntax(option)),
                                new Div { Margin = new Thickness(1, 0, 1, 1), Color = Gray }
                                    .AddChildren(option.HelpText),
                            })
                        )
                );
        }

        private static object GetOptionSyntax (BaseOptionAttribute option)
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