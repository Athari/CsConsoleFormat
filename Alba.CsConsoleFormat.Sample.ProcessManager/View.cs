using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using static System.ConsoleColor;

// ReSharper disable AnnotateCanBeNullParameter
// ReSharper disable AnnotateNotNullParameter
namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal static class View
    {
        private static readonly LineThickness StrokeHeader = new LineThickness(LineWidth.None, LineWidth.Double);
        private static readonly LineThickness StrokeRight = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);

        public static Document Error(string message, string extra = null) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Span("Error\n") { Color = Red },
                    new Span(message) { Color = White },
                    extra != null ? $"\n\n{extra}" : null
                }
            };

        public static Document Info(string message) =>
            new Document {
                Background = Black, Color = Gray,
                Children = { message }
            };

        public static Document ProcessList(IEnumerable<Process> processes) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Grid {
                        Stroke = StrokeHeader, StrokeColor = DarkGray,
                        Columns = {
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Auto, MaxWidth = 20 },
                            new Column { Width = GridLength.Star(1) },
                            new Column { Width = GridLength.Auto }
                        },
                        Children = {
                            new Cell("Id") { Stroke = StrokeHeader, Color = White },
                            new Cell("Name") { Stroke = StrokeHeader, Color = White },
                            new Cell("Main Window Title") { Stroke = StrokeHeader, Color = White },
                            new Cell("Private Memory") { Stroke = StrokeHeader, Color = White },
                            processes.Select(process => new[] {
                                new Cell {
                                    Stroke = StrokeRight,
                                    Children = { process.Id }
                                },
                                new Cell {
                                    Stroke = StrokeRight, Color = Yellow, TextWrap = TextWrap.NoWrap,
                                    Children = { process.ProcessName }
                                },
                                new Cell {
                                    Stroke = StrokeRight, Color = White, TextWrap = TextWrap.NoWrap,
                                    Children = { process.MainWindowTitle }
                                },
                                new Cell {
                                    Stroke = LineThickness.None, Align = Align.Right,
                                    Children = { process.PrivateMemorySize64.ToString("n0") }
                                },
                            })
                        }
                    }
                }
            };

        public static Document HelpOptionsList(IEnumerable<BaseOptionAttribute> options, string instruction) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Div(instruction) { Color = White },
                    "",
                    new Grid {
                        Stroke = LineThickness.None,
                        Columns = { GridLength.Auto, GridLength.Star(1) },
                        Children = { options.Select(OptionNameAndHelp) }
                    }
                }
            };

        public static Document HelpAllOptionsList(ILookup<BaseOptionAttribute, BaseOptionAttribute> verbsWithOptions, string instruction) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Span($"{instruction}\n") { Color = White },
                    new Grid {
                        Stroke = LineThickness.None,
                        Columns = { GridLength.Auto, GridLength.Star(1) },
                        Children = {
                            verbsWithOptions.Select(verbWithOptions => new object[] {
                                OptionNameAndHelp(verbWithOptions.Key),
                                new Grid {
                                    Stroke = LineThickness.None, Margin = new Thickness(4, 0, 0, 0),
                                    [Grid.ColumnSpanProperty] = 2,
                                    Columns = { GridLength.Auto, GridLength.Star(1) },
                                    Children = { verbWithOptions.Select(OptionNameAndHelp) }
                                }
                            })
                        }
                    }
                }
            };

        private static object[] OptionNameAndHelp(BaseOptionAttribute option) => new object[] {
            new Div {
                Margin = new Thickness(1, 0, 1, 1), Color = Yellow, MinWidth = 14,
                Children = { GetOptionSyntax(option) }
            },
            new Div {
                Margin = new Thickness(1, 0, 1, 1),
                Children = { option.HelpText }
            },
        };

        private static object GetOptionSyntax(BaseOptionAttribute option)
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