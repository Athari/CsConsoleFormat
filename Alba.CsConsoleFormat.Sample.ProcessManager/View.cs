using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using static System.ConsoleColor;

namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal class View
    {
        private static readonly LineThickness StrokeHeader = new LineThickness(LineWidth.None, LineWidth.Wide);
        private static readonly LineThickness StrokeRight = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);

        public Document Error (string message, string extra = null) =>
            new Document { Background = Black, Color = Gray }
                .AddChildren(
                    new Span("Error\n") { Color = Red },
                    new Span(message) { Color = White },
                    extra != null ? $"\n\n{extra}" : null
                );

        public Document Info (string message) =>
            new Document { Background = Black, Color = Gray }
                .AddChildren(message);

        public Document ProcessList (IEnumerable<Process> processes) =>
            new Document { Background = Black, Color = Gray }
                .AddChildren(
                    new Grid { Stroke = StrokeHeader, StrokeColor = DarkGray }
                        .AddColumns(
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Auto, MaxWidth = 20 },
                            new Column { Width = GridLength.Star(1) },
                            new Column { Width = GridLength.Auto }
                        )
                        .AddChildren(
                            new Cell { Stroke = StrokeHeader, Color = White }
                                .AddChildren("Id"),
                            new Cell { Stroke = StrokeHeader, Color = White }
                                .AddChildren("Name"),
                            new Cell { Stroke = StrokeHeader, Color = White }
                                .AddChildren("Main Window Title"),
                            new Cell { Stroke = StrokeHeader, Color = White }
                                .AddChildren("Private Memory"),
                            processes.Select(process => new[] {
                                new Cell { Stroke = StrokeRight }
                                    .AddChildren(process.Id),
                                new Cell { Stroke = StrokeRight, Color = Yellow, TextWrap = TextWrapping.NoWrap }
                                    .AddChildren(process.ProcessName),
                                new Cell { Stroke = StrokeRight, Color = White, TextWrap = TextWrapping.NoWrap }
                                    .AddChildren(process.MainWindowTitle),
                                new Cell { Stroke = LineThickness.None, Align = HorizontalAlignment.Right }
                                    .AddChildren(process.PrivateMemorySize64.ToString("n0")),
                            })
                        )
                );

        public Document HelpOptionsList (IEnumerable<BaseOptionAttribute> options, string instruction) =>
            new Document { Background = Black, Color = Gray }
                .AddChildren(
                    new Div { Color = White }
                        .AddChildren(instruction),
                    "",
                    new Grid { Stroke = LineThickness.None }
                        .AddColumns(GridLength.Auto, GridLength.Star(1))
                        .AddChildren(options.Select(OptionNameAndHelp))
                );

        public Document HelpAllOptionsList (ILookup<BaseOptionAttribute, BaseOptionAttribute> verbsWithOptions, string instruction) =>
            new Document { Background = Black, Color = Gray }
                .AddChildren(
                    new Span($"{instruction}\n") { Color = White },
                    new Grid { Stroke = LineThickness.None }
                        .AddColumns(GridLength.Auto, GridLength.Star(1))
                        .AddChildren(
                            verbsWithOptions.Select(verbWithOptions => new object[] {
                                OptionNameAndHelp(verbWithOptions.Key),
                                new Grid { Stroke = LineThickness.None, Margin = new Thickness(4, 0, 0, 0) }
                                    .Set(Grid.ColumnSpanProperty, 2)
                                    .AddColumns(GridLength.Auto, GridLength.Star(1))
                                    .AddChildren(verbWithOptions.Select(OptionNameAndHelp)),
                            })
                        )
                );

        private static object[] OptionNameAndHelp (BaseOptionAttribute option) => new[] {
            new Div { Margin = new Thickness(1, 0, 1, 1), Color = Yellow, MinWidth = 14 }
                .AddChildren(GetOptionSyntax(option)),
            new Div { Margin = new Thickness(1, 0, 1, 1) }
                .AddChildren(option.HelpText),
        };

        private static object GetOptionSyntax (BaseOptionAttribute option)
        {
            if (option is VerbOptionAttribute)
                return option.LongName;
            return option.ShortName == null
                ? $"--{option.LongName}"
                : option.LongName == null
                    ? $"-{option.ShortName}"
                    : $"--{option.LongName}, -{option.ShortName}";
        }
    }
}