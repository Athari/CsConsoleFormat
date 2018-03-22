using System;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionMetaAttribute : Attribute
    {
        public string MetaName { get; set; }
        public string MetaValue { get; set; }
        public string HelpText { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
    }
}