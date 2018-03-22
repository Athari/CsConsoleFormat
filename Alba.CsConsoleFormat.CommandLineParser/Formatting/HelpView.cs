using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
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
        public Thickness ErrorMargin { get; set; } = new Thickness(0, 1, 0, 0);
        public Thickness ExampleGroupMargin { get; set; } = new Thickness(0, 0, 0, 0);
        public Thickness ExampleMargin { get; set; } = new Thickness(2, 0, 0, 0);
        public Thickness OptionMargin { get; set; } = new Thickness(2, 1, 0, 0);
        public Thickness SubOptionMargin { get; set; } = new Thickness(6, 1, 0, 0);
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
        public ConsoleColor? AssemblyMetaColor { get; set; }
        public ConsoleColor? TitleColor { get; set; }
        public ConsoleColor? UsageTextColor { get; set; }
        public ConsoleColor? UsageExampleTextColor { get; set; }
        public ConsoleColor? UsageExampleAppColor { get; set; }
        public ConsoleColor? UsageExampleOptionsColor { get; set; }
        public ConsoleColor? ErrorTitleColor { get; set; }
        public ConsoleColor? ErrorTextColor { get; set; }
        public ConsoleColor? ErrorDetailColor { get; set; }
        public ConsoleColor? WarningTitleColor { get; set; }
        public ConsoleColor? InfoTitleColor { get; set; }
        public ConsoleColor? OptionNameColor { get; set; }
        public ConsoleColor? OptionTextColor { get; set; }
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;
        public CultureInfo Culture { get; set; }
        public string ListIndexFormat { get; set; } = "- ";
        public int OptionNameMinLength { get; set; } = 16;
        public int OptionNameMaxLength { get; set; } = 24;
        public Func<string, string> TitleTransform = s => s.ToUpper();

        public HelpView(bool useDefaultColors = true)
        {
            if (useDefaultColors) {
                AssemblyMetaColor = ConsoleColor.DarkGray;
                TitleColor = ConsoleColor.White;
                UsageExampleAppColor = ConsoleColor.Green;
                UsageExampleOptionsColor = ConsoleColor.Yellow;
                ErrorTitleColor = ConsoleColor.Red;
                ErrorDetailColor = ConsoleColor.DarkGray;
                WarningTitleColor = ConsoleColor.Yellow;
                InfoTitleColor = ConsoleColor.Cyan;
                OptionNameColor = ConsoleColor.Yellow;
            }
        }

        internal CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;

        [Pure]
        public virtual Document Help(HelpParts parts)
        {
            return new Document {
                Color = Color,
                Background = Background,
                Language = new XmlLanguage(EffectiveCulture),
                Margin = Margin,
                Children = {
                    GetHeader(parts),
                    GetErrorsCore(parts),
                    GetUsageCore(parts),
                    GetOptionsCore(parts),
                    GetExamplesCore(parts),
                    GetFooter(parts),
                }
            };
        }

        [Pure]
        public virtual Document HelpNotParsed(
            object errorsSource = null,
            HelpParts partsErrors = HelpParts.DefaultErrors,
            HelpParts partsOptions = HelpParts.DefaultOptions,
            HelpParts partsVersion = HelpParts.DefaultVersion)
        {
            if (errorsSource != null)
                return With(errorsSource: errorsSource).HelpNotParsed(null, partsErrors, partsOptions, partsVersion);
            if (!Info.Parts.Has(HelpParts.Errors))
                return Help(HelpParts.None);
            if (Info.Errors.Any(e => e.Kind == ErrorKind.ParseError))
                return Help(partsErrors);
            if (Info.Errors.Any(e => e.Kind == ErrorKind.VersionVerb))
                return Help(partsVersion);
            if (Info.Errors.Any(e => e.Kind == ErrorKind.HelpVerb)) {
                var helpError = Info.Errors.First(e => e.Kind == ErrorKind.HelpVerb);
                if (helpError.TypeKey != null)
                    return With(optionsSource: new[] { helpError.TypeKey }).Help((partsOptions | HelpParts.SubOptions) & ~HelpParts.BuiltInOptions);
            }
            return Help(partsErrors);
        }

        [Pure]
        private Document Message(object errorsSource, HelpParts parts = HelpParts.Errors) => With(errorsSource: errorsSource).Help(parts);

        [Pure]
        public Document Message(ErrorInfo error, HelpParts parts = HelpParts.Errors) => Message((object)error, parts);
        
        [Pure]
        public Document Message(IEnumerable<ErrorInfo> errors, HelpParts parts = HelpParts.Errors) => Message((object)errors, parts);

        [Pure]
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

        [Pure]
        protected virtual object GetFooter(HelpParts parts)
        {
            return new Div(Footer);
        }

        protected virtual Assembly EffectiveAssembly => AssemblySource ?? Assembly.GetEntryAssembly();

        [Pure]
        private object GetAssemblyTitleCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyTitle | HelpParts.AssemblyVersion))
                return null;
            return GetAssemblyTitle(parts);
        }

        [Pure]
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

        [Pure]
        private object GetAssemblyCopyrightCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyCopyright))
                return null;
            return GetAssemblyCopyright(parts);
        }

        [Pure]
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

        [Pure]
        private object GetAssemblyLicenseCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyLicense))
                return null;
            return GetAssemblyLicense(parts);
        }

        [Pure]
        protected virtual object GetAssemblyLicense(HelpParts parts)
        {
            return new Div(ClpUtils.GetAssemblyLicenseText(EffectiveAssembly));
        }

        [Pure]
        private object GetUsageCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.AssemblyUsage))
                return null;
            return GetUsage(parts);
        }

        [Pure]
        protected virtual object GetUsage(HelpParts parts)
        {
            string usage = ClpUtils.GetAssemblyUsageText(EffectiveAssembly);
            if (usage == "")
                return null;
            return new Div {
                Margin = SectionMargin,
                Children = {
                    new Div(TitleTransform("Usage")) { Color = TitleColor },
                    new Div(usage) { Color = UsageTextColor },
                }
            };
        }

        [Pure]
        private object GetExamplesCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.Examples) || !Info.Parts.Has(HelpParts.Examples) || !Info.Examples.SelectMany(e => e).Any())
                return null;
            return GetExamples(parts);
        }

        [Pure]
        protected virtual object GetExamples(HelpParts parts)
        {
            return new Div {
                Margin = SectionMargin,
                Children = {
                    new Div(TitleTransform("Examples")) { Color = TitleColor },
                    new List {
                        IndexFormat = new string('\n', ExampleGroupMargin.Top) + ListIndexFormat,
                        Children = {
                            Info.Examples.SelectMany(e => e).Select(GetExample)
                        }
                    }
                }
            };
        }

        [Pure]
        protected virtual Div GetExample(ExampleInfo example)
        {
            string appName = EffectiveAssembly?.GetName().Name;
            return new Div {
                Margin = ExampleGroupMargin,
                Children = {
                    new Div(example.HelpText) { Color = UsageExampleTextColor },
                    example.SampleTexts.Select(t => new Div {
                        Margin = ExampleMargin,
                        TextWrap = TextWrap.WordWrapSpace,
                        Children = {
                            new Span(example.AppName ?? appName ?? "*") { Color = UsageExampleAppColor },
                            " ",
                            new Span(t) { Color = UsageExampleOptionsColor }
                        }
                    })
                }
            };
        }

        [Pure]
        private object GetErrorsCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.Errors) || !Info.Parts.Has(HelpParts.Errors) || Info.Errors.All(e => e.Message == null))
                return null;
            return GetErrors(parts);
        }

        [Pure]
        protected virtual object GetErrors(HelpParts parts)
        {
            return Info.Errors.Where(e => e.Message != null).Select(GetError);
        }

        [Pure]
        private object GetOptionsCore(HelpParts parts)
        {
            if (!parts.Has(HelpParts.Options) || !Info.Parts.Has(HelpParts.Options) || !Info.Options.Any(IsOptionVisible(parts)))
                return null;
            return GetOptions(parts);
        }

        [Pure]
        protected virtual object GetOptions(HelpParts parts)
        {
            return new Grid {
                Stroke = LineThickness.None,
                Columns = {
                    new Column {
                        Width = GridLength.Auto,
                        MinWidth = OptionNameMinLength + OptionMargin.Left + 1,
                        MaxWidth = OptionNameMaxLength + OptionMargin.Left + 1,
                    },
                    new Column { Width = GridLength.Star(1) },
                },
                Children = {
                    Info.Options.Where(IsOptionVisible(parts)).Select(option => GetOption(option, parts, isSubOption: false))
                }
            };
        }

        [Pure]
        protected virtual object GetOption(OptionInfo option, HelpParts parts, bool isSubOption)
        {
            var m = isSubOption ? SubOptionMargin : OptionMargin;
            var marginName = new Thickness(m.Left, m.Top, 1, m.Bottom);
            var marginText = new Thickness(0, m.Top, m.Right, m.Bottom);

            string values = parts.Has(HelpParts.OptionsValues) && option.HasHelpValues
                ? string.Join(", ", option.HelpValues) : null;
            string defaultValue = parts.Has(HelpParts.OptionsDefaultValue) && option.DefaultValue != null
                ? GetOptionDefault(option) : null;
            object subOptions = parts.Has(HelpParts.SubOptions) && option.SubOptions.Any(IsOptionVisible(parts))
                ? GetSubOptions(option, parts) : null;

            return new[] {
                new Div {
                    Margin = marginName,
                    Color = OptionNameColor,
                    Children = { option.HelpName }
                },
                new Div {
                    Margin = marginText,
                    Color = OptionTextColor,
                    Children = {
                        option.HelpText,
                        option.IsRequired ? " (Required)" : null,
                        values != null ? $"\nValid values: {values}" : null,
                        defaultValue != null ? $"\nDefault: {defaultValue}" : null
                    }
                },
                subOptions != null
                    ? new Div {
                        [Grid.ColumnSpanProperty] = 2,
                        Children = { subOptions }
                    }
                    : null
            };
        }

        protected virtual object GetSubOptions(OptionInfo option, HelpParts parts)
        {
            return new Grid {
                Stroke = LineThickness.None,
                Columns = {
                    new Column {
                        Width = GridLength.Auto,
                        MinWidth = OptionNameMinLength + SubOptionMargin.Left + 1,
                        MaxWidth = OptionNameMaxLength + SubOptionMargin.Left + 1,
                    },
                    new Column { Width = GridLength.Star(1) },
                },
                Children = {
                    option.SubOptions.Where(IsOptionVisible(parts)).Select(o => GetOption(o, parts, isSubOption: true))
                }
            };
        }

        protected virtual string GetOptionDefault(OptionInfo option)
        {
            switch (option.DefaultValue) {
                case null:
                    return null;
                case bool b:
                    return ToString(b).ToLower();
                case string s:
                    return s;
                case IEnumerable e:
                    return string.Join("  ", e);
                default:
                    return null;
            }

            string ToString(object value) => Convert.ToString(value, EffectiveCulture);
        }

        protected virtual object GetError(ErrorInfo error)
        {
            bool displayInfoTitle = false;
            return new Div {
                Margin = ErrorMargin,
                Children = {
                    error.Kind == ErrorKind.Error
                        ? new Div(TitleTransform("Error")) { Color = ErrorTitleColor }
                        : null,
                    error.Kind == ErrorKind.Warning
                        ? new Div(TitleTransform("Warning")) { Color = WarningTitleColor }
                        : null,
                    // ReSharper disable once ExpressionIsAlwaysNull
                    error.Kind == ErrorKind.Info && displayInfoTitle
                        ? new Div(TitleTransform("Information")) { Color = InfoTitleColor }
                        : null,
                    new Div(error.Message) { Color = ErrorTextColor },
                    new Div(error.Details) { Color = ErrorDetailColor }
                }
            };
        }

        internal HelpView With(ICollection<Type> optionsSource = null, object errorsSource = null)
        {
            var copy = (HelpView)MemberwiseClone();
            copy.Info = copy.Info.With(optionsSource, errorsSource);
            return copy;
        }

        private static Func<OptionInfo, bool> IsOptionVisible(HelpParts parts) =>
            option => (option.IsVisible || parts.Has(HelpParts.HiddenOptions)) && (!option.IsBuiltIn || parts.Has(HelpParts.BuiltInOptions));
    }
}