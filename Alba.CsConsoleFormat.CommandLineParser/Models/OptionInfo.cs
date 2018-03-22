using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public sealed partial class OptionInfo
    {
        private static readonly IList<OptionInfo> EmptySubOptions = new OptionInfo[0];

        [CanBeNull]
        private List<Attribute> _attributes;
        private List<OptionInfo> _subOptions;

        public bool IsPositional { get; private set; }

        public bool IsRequired { get; private set; }

        public bool IsVisible { get; private set; }

        public int Index { get; private set; }

        public object DefaultValue { get; private set; }

        [CanBeNull]
        public string HelpText { get; private set; }

        [CanBeNull]
        public string MetaValue { get; private set; }

        [CanBeNull]
        public string Name { get; private set; }

        [CanBeNull]
        public string ShortName { get; private set; }

        [CanBeNull]
        public string SetName { get; private set; }

        public ValueKind ValueKind { get; private set; }

        internal MemberInfo SourceMember { get; private set; }

        private OptionInfo()
        { }

        public IList<OptionInfo> SubOptions => new ReadOnlyCollection<OptionInfo>(_subOptions ?? EmptySubOptions);

        internal static OptionInfo FromMember(MemberInfo member, IEnumerable<Attribute> attributes, bool isVerb = false)
        {
          #if CLP_19
            if (ClpUtils.IsVersion19)
                return new OptionInfo().FromMember19(member, attributes, isVerb);
          #endif
          #if CLP_22
            if (ClpUtils.IsVersion22)
                return new OptionInfo().FromMember22(member, attributes, isVerb);
          #endif
            throw ClpUtils.UnsupportedVersion();
        }

        internal OptionInfo WithSubOptions(IEnumerable<OptionInfo> subOptions)
        {
            _subOptions = new List<OptionInfo>(subOptions);
            return this;
        }

        [CanBeNull]
        private TAttribute Get<TAttribute>() where TAttribute : Attribute =>
            _attributes?.OfType<TAttribute>().FirstOrDefault();

        private static string Nullable(string value) => string.IsNullOrEmpty(value) ? null : value;

        public override string ToString() => $"{Name}{(IsPositional ? $"#{Index}" : "")}";
    }
}