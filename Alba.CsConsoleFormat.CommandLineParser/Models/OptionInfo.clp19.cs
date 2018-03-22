extern alias CommandLineParser_1_9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineParser_1_9::CommandLine;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public partial class OptionInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private OptionInfo FromMember19(MemberInfo member, IEnumerable<Attribute> attributes, bool isVerb = false)
        {
            _attributes = attributes.Where(a => a is BaseOptionAttribute || a is ValueOptionAttribute).ToList();
            SourceMember = member;

            var option = Get<BaseOptionAttribute>();
            var value = Get<ValueOptionAttribute>();

            IsPositional = value != null;
            IsRequired = !isVerb && (IsPositional || option?.Required == true);
            IsVisible = true;
            Index = value?.Index ?? -1;
            DefaultValue = option?.DefaultValue;
            HelpText = Nullable(option?.HelpText);
            MetaValue = null;
            Name = Nullable(option?.LongName) ?? member?.Name.ToLower();
            ShortName = Nullable(option?.ShortName?.ToString());
            SetName = Nullable(option?.MutuallyExclusiveSet);
            OptionKind = isVerb ? OptionKind.Verb : GetValueKind19(option, value);

            return this;
        }

        private static OptionKind GetValueKind19(BaseOptionAttribute option, ValueOptionAttribute value)
        {
            switch (option) {
                case OptionAttribute _:
                case HelpOptionAttribute _:
                    return OptionKind.Single;
                case OptionArrayAttribute _:
                    return OptionKind.Array;
                case OptionListAttribute _:
                    return OptionKind.List;
                case VerbOptionAttribute _:
                case HelpVerbOptionAttribute _:
                    return OptionKind.Verb;
                default:
                    return value != null ? OptionKind.Single : OptionKind.Unknown;
            }
        }
    }
}