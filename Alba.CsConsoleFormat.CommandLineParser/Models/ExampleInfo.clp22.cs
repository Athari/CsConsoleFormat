extern alias CommandLineParser_2_2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommandLineParser_2_2::CommandLine;
using CommandLineParser_2_2::CommandLine.Text;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public partial class ExampleInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private ExampleInfo FromExample22(object data)
        {
            var example = (Example)data;
            HelpText = example.HelpText;
            List<UnParserSettings> styles = example.FormatStyles.ToList();
            if (styles.Count == 0)
                styles.Add(new UnParserSettings());
            SampleTexts = styles.Select(s => Parser.Default.FormatCommandLine(example.Sample,
                config => {
                    config.PreferShortName = s.PreferShortName;
                    config.GroupSwitches = s.GroupSwitches;
                    config.UseEqualToken = s.UseEqualToken;
                }))
                .ToList();
            return this;
        }
    }
}
