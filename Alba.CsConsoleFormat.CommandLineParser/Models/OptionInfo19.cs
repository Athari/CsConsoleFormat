extern alias CommandLineParser_1_9;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommandLineParser_1_9::CommandLine;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    internal sealed class OptionInfo19 : OptionInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public OptionInfo19(IEnumerable<Attribute> attributes, string defaultName, bool isVerb = false) : base(attributes)
        {
            var option = Get<BaseOptionAttribute>();
            var value = Get<ValueOptionAttribute>();

            IsPositional = value != null;
            IsRequired = !isVerb && (IsPositional || option?.Required == true);
            IsVisible = true;
            Index = value?.Index ?? -1;
            DefaultValue = option?.DefaultValue;
            HelpText = Nullable(option?.HelpText);
            MetaValue = null;
            Name = Nullable(option?.LongName ?? defaultName?.ToLower());
            ShortName = Nullable(option?.ShortName?.ToString());
            SetName = Nullable(option?.MutuallyExclusiveSet);
            ValueKind = isVerb ? ValueKind.Verb : GetValueKind(option, value);
        }

        private static ValueKind GetValueKind(BaseOptionAttribute option, ValueOptionAttribute value)
        {
            switch (option) {
                case OptionAttribute _:
                case HelpOptionAttribute _:
                    return ValueKind.Single;
                case OptionArrayAttribute _:
                    return ValueKind.Array;
                case OptionListAttribute _:
                    return ValueKind.List;
                case VerbOptionAttribute _:
                case HelpVerbOptionAttribute _:
                    return ValueKind.Verb;
                default:
                    return value != null ? ValueKind.Single : ValueKind.Unknown;
            }
        }

        protected override bool IsAttributeSupported(Attribute attr) =>
            attr is BaseOptionAttribute || attr is ValueOptionAttribute;
    }
}