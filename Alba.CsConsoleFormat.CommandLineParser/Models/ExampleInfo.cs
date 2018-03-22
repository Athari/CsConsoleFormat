using System.Collections.Generic;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public sealed partial class ExampleInfo
    {
        public string AppName { get; private set; }
        public string HelpText { get; private set; }
        public IList<string> SampleTexts { get; private set; }

        private ExampleInfo()
        { }

        internal static ExampleInfo FromExample(object source, string appName)
        {
            var example = new ExampleInfo { AppName = appName };
          #if CLP_22
            if (ClpUtils.IsVersion22)
                return example.FromExample22(source);
          #endif
            throw ClpUtils.UnsupportedVersion();
        }
    }
}