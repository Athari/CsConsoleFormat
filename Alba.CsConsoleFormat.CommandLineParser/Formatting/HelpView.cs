using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if NET_40 || NET_STANDARD_15
using Alba.CsConsoleFormat.CommandLineParser.Framework;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public class HelpView
    {
        public HelpInfo Info { get; private set; } = new HelpInfo();
        public Assembly AssemblySource { get; set; }
        public object Header { get; set; }
        public object Footer { get; set; }

        public Thickness Margin { get; set; } = new Thickness(0);
        public Thickness SectionMargin { get; set; } = new Thickness(0, 1, 0, 0);
        public Thickness ExampleGroupMargin { get; set; } = new Thickness(0, 0, 0, 0);
        public Thickness ExampleMargin { get; set; } = new Thickness(2, 0, 0, 0);
        public Thickness VerbMargin { get; set; } = new Thickness(0);
        public Thickness OptionMargin { get; set; } = new Thickness(0);
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
        public ConsoleColor? AssemblyMetaColor { get; set; }
        public ConsoleColor? TitleColor { get; set; }
        public ConsoleColor? UsageTextColor { get; set; }
        public ConsoleColor? UsageExampleTextColor { get; set; }
        public ConsoleColor? UsageExampleAppColor { get; set; }
        public ConsoleColor? UsageExampleOptionsColor { get; set; }
        public ConsoleColor? ErrorColor { get; set; }
        public ConsoleColor? ErrorDetailColor { get; set; }
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;
        public string ListIndexFormat { get; set; } = "- ";

        public HelpView(bool useDefaultColors = true)
        {
            if (useDefaultColors) {
                AssemblyMetaColor = ConsoleColor.DarkGray;
                TitleColor = ConsoleColor.White;
                UsageExampleAppColor = ConsoleColor.Green;
                UsageExampleOptionsColor = ConsoleColor.Yellow;
                ErrorColor = ConsoleColor.Red;
            }
        }

        public virtual Document Help(HelpParts parts)
        {
            return new Document {
                Color = Color,
                Background = Background,
                Margin = Margin,
                Children = {
                    GetHeader(parts),
                    GetUsageCore(parts),
                    GetUsageExamplesCore(parts),
                    GetErrorsCore(parts),
                    GetFooter(parts),
                }
            };
        }

        public virtual Document HelpError(
            HelpParts partsErrors = HelpParts.DefaultErrors,
            HelpParts partsVersion = HelpParts.DefaultVersion,
            HelpParts partsOptions = HelpParts.DefaultOptions)
        {
            if (!Info.Parts.Has(HelpParts.Errors))
                return Help(HelpParts.None);
            if (Info.Errors.Any(e => e.Kind == ErrorKind.Version))
                return Help(partsVersion);
            if (Info.Errors.Any(e => e.Kind == ErrorKind.Help)) {
                var helpError = Info.Errors.First(e => e.Kind == ErrorKind.Help);
                if (helpError.TypeKey != null) {
                    HelpView verbHelpView = Clone();
                    verbHelpView.Info = new HelpInfo { OptionsSource = { helpError.TypeKey } };
                    return verbHelpView.Help(partsOptions & ~HelpParts.Errors);
                }
            }
            return Help(partsErrors);
        }

        public Document HelpError(object errorSource, HelpParts parts = HelpParts.DefaultErrors)
        {
            HelpView errorHelpView = Clone();
            errorHelpView.Info = new HelpInfo { ErrorsSource = errorSource };
            return errorHelpView.Help(parts);
        }

        protected virtual object GetHeader(HelpParts parts)
        {
            return new Div {
                Color = AssemblyMetaColor,
                Children = {
                    GetAssemblyTitleCore(parts),
                    GetAssemblyCopyrightCore(parts),
                    GetAssemblyLicenseCore(parts),
                    Header,
                }
            };
        }

        protected virtual object GetFooter(HelpParts parts)
        {
            return new Div(Footer);
        }

        protected virtual Assembly EffectiveAssembly => AssemblySource ?? Assembly.GetEntryAssembly();

        private object GetAssemblyTitleCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyTitle | HelpParts.AssemblyVersion))
                return null;
            return GetAssemblyTitle(parts);
        }

        protected virtual object GetAssemblyTitle(HelpParts parts)
        {
            string title = EffectiveAssembly?.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault()?.Title
             ?? EffectiveAssembly?.GetName().Name;
            string version = EffectiveAssembly?.GetCustomAttributes<AssemblyInformationalVersionAttribute>().FirstOrDefault()?.InformationalVersion
             ?? EffectiveAssembly?.GetName().Version.ToString();
            string line = string.Join(" ",
                parts.Has(HelpParts.AssemblyTitle) ? title : null,
                parts.Has(HelpParts.AssemblyVersion) ? version : null).Trim();
            return new Div(line);
        }

        private object GetAssemblyCopyrightCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyCopyright))
                return null;
            return GetAssemblyCopyright(parts);
        }

        protected virtual object GetAssemblyCopyright(HelpParts parts)
        {
            string copyright = EffectiveAssembly?.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault()?.Copyright;
            if (copyright == null) {
                string company = EffectiveAssembly?.GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault()?.Company;
                if (company == null)
                    return null;
                copyright = $"{company} © {DateTime.UtcNow.Year}";
            }
            if (copyright.IndexOf("Copyright", StringComparison.OrdinalIgnoreCase) == -1)
                copyright = $"Copyright {copyright}";
            return new Div(copyright);
        }

        private object GetAssemblyLicenseCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyLicense))
                return null;
            return GetAssemblyLicense(parts);
        }

        protected virtual object GetAssemblyLicense(HelpParts parts)
        {
            return new Div(ClpUtils.GetAssemblyLicenseText(EffectiveAssembly));
        }

        private object GetUsageCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyUsage))
                return null;
            return GetUsage(parts);
        }

        protected virtual object GetUsage(HelpParts parts)
        {
            string usage = ClpUtils.GetAssemblyUsageText(EffectiveAssembly);
            if (usage == "")
                return null;
            return new Div {
                Margin = SectionMargin,
                Children = {
                    new Div("USAGE:") { Color = TitleColor },
                    new Div(usage) { Color = UsageTextColor },
                }
            };
        }

        private object GetUsageExamplesCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.Examples) || !Info.Parts.Has(HelpParts.Examples) || !Info.Examples.SelectMany(e => e).Any())
                return null;
            return GetUsageExamples(parts);
        }

        protected virtual object GetUsageExamples(HelpParts parts)
        {
            return new Div {
                Margin = SectionMargin,
                Children = {
                    new Div("EXAMPLES:") { Color = TitleColor },
                    new List {
                        IndexFormat = new string('\n', ExampleGroupMargin.Top) + ListIndexFormat,
                        Children = {
                            Info.Examples.SelectMany(e => e).Select(GetUsageExample)
                        }
                    }
                }
            };
        }

        protected virtual Div GetUsageExample(ExampleInfo example)
        {
            string appName = EffectiveAssembly?.GetName().Name;
            return new Div {
                Margin = ExampleGroupMargin,
                Children = {
                    new Div(example.HelpText) { Color = UsageExampleTextColor },
                    example.SampleTexts.Select(t => new Div {
                        Margin = ExampleMargin,
                        Children = {
                            new Span(example.AppName ?? appName ?? "*") { Color = UsageExampleAppColor },
                            " ",
                            new Span(t) { Color = UsageExampleOptionsColor }
                        }
                    })
                }
            };
        }

        private object GetErrorsCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.Errors) || !Info.Parts.Has(HelpParts.Errors) || Info.Errors.All(e => e.Message == null))
                return null;
            return GetErrors(parts);
        }

        protected virtual object GetErrors(HelpParts parts)
        {
            string errorsTitle = "ERROR" + (Info.Errors.Count > 1 ? "S" : "") + ":";
            return new Div {
                Margin = SectionMargin,
                Children = {
                    new Div(errorsTitle) { Color = TitleColor },
                    new List {
                        IndexFormat = ListIndexFormat,
                        Children = {
                            Info.Errors.Where(e => e.Message != null).Select(GetError)
                        }
                    }
                }
            };
        }

        protected virtual object GetError(ErrorInfo error)
        {
            return new[] {
                new Div(error.Message) { Color = ErrorColor },
                new Div(error.Details) { Color = ErrorDetailColor }
            };
        }

        internal HelpView Clone() => (HelpView)MemberwiseClone();
    }
}