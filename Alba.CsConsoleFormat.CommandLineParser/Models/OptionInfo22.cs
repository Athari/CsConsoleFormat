extern alias CommandLineParser_2_2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommandLineParser_2_2::CommandLine;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    internal sealed class OptionInfo22 : OptionInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public OptionInfo22(IEnumerable<Attribute> attributes, string defaultName, bool isVerb = false) : base(attributes)
        {
            var basic = Get<BaseAttribute>();
            var option = Get<OptionAttribute>();
            var value = Get<ValueAttribute>();
            var verb = Get<VerbAttribute>();

            IsPositional = value != null;
            IsRequired = !isVerb && verb == null && (basic?.Required ?? false);
            IsVisible = !(verb?.Hidden == true || basic?.Hidden == true);
            Index = value?.Index ?? -1;
            DefaultValue = basic?.Default;
            HelpText = Nullable(verb?.HelpText ?? basic?.HelpText);
            MetaValue = Nullable(basic?.MetaValue);
            Name = Nullable(verb?.Name ?? value?.MetaName ?? option?.LongName ?? defaultName?.ToLower());
            ShortName = Nullable(option?.ShortName);
            SetName = Nullable(option?.SetName);
            ValueKind = isVerb ? ValueKind.Verb : GetValueKind(option, value, verb);
        }

        private static ValueKind GetValueKind(OptionAttribute option, ValueAttribute value, VerbAttribute verb)
        {
            if (verb != null)
                return ValueKind.Verb;
            if (option != null)
                return option.Separator == '\0' ? ValueKind.Single : ValueKind.List;
            if (value != null)
                return ValueKind.Single;
            return ValueKind.Unknown;
        }

        protected override bool IsAttributeSupported(Attribute attr) =>
            attr is BaseAttribute || attr is VerbAttribute;
    }
}