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
            _attributes = attributes.Where(a =>
                a is BaseOptionAttribute || a is ValueOptionAttribute || a is ValueListAttribute
             || a is OptionMetaAttribute || a is ParserStateAttribute).ToList();
            SourceMember = member;

            if (Get<ParserStateAttribute>() != null)
                return null;

            var option = Get<BaseOptionAttribute>();
            var value = Get<ValueOptionAttribute>();
            var valueList = Get<ValueListAttribute>();
            var meta = Get<OptionMetaAttribute>();

            IsPositional = value != null;
            IsRequired = !isVerb && (option?.Required == true || meta?.Required == true);
            IsVisible = meta?.Hidden != true;
            Index = value?.Index ?? -1;
            DefaultValue = option?.DefaultValue;
            HelpText = Nullable(option?.HelpText) ?? Nullable(meta?.HelpText);
            MetaValue = Nullable(option?.MetaValue);
            ShortName = Nullable(option?.ShortName?.ToString());
            Name = Nullable(option?.LongName) ?? Nullable(meta?.MetaName) ?? (ShortName != null ? null : member?.Name.ToLower());
            SetName = Nullable(option?.MutuallyExclusiveSet);
            OptionKind = isVerb ? OptionKind.Verb : GetValueKind19(option, value, valueList);

            return this;
        }

        private static OptionKind GetValueKind19(BaseOptionAttribute option, ValueOptionAttribute value, ValueListAttribute valueList)
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
                    if (value != null)
                        return OptionKind.Single;
                    else if (valueList != null)
                        return OptionKind.List;
                    else
                        return OptionKind.Unknown;
            }
        }
    }
}