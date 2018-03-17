using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public abstract class OptionInfo
    {
        private static readonly IList<OptionInfo> EmptySubOptions = new OptionInfo[0];

        private readonly List<Attribute> _attributes;
        private List<OptionInfo> _subOptions;

        public bool IsPositional { get; protected set; }

        public bool IsRequired { get; protected set; }

        public bool IsVisible { get; protected set; }

        public int Index { get; protected set; }

        public object DefaultValue { get; protected set; }

        [CanBeNull]
        public string HelpText { get; protected set; }

        [CanBeNull]
        public string MetaValue { get; protected set; }

        [CanBeNull]
        public string Name { get; protected set; }

        [CanBeNull]
        public string ShortName { get; protected set; }

        [CanBeNull]
        public string SetName { get; protected set; }

        public ValueKind ValueKind { get; protected set; }

        protected OptionInfo(IEnumerable<Attribute> attributes)
        {
            _attributes = attributes.Where(IsAttributeSupported).ToList();
        }

        public IList<OptionInfo> SubOptions => new ReadOnlyCollection<OptionInfo>(_subOptions ?? EmptySubOptions);

        internal OptionInfo WithSubOptions(IEnumerable<OptionInfo> subOptions)
        {
            _subOptions = new List<OptionInfo>(subOptions);
            return this;
        }

        protected abstract bool IsAttributeSupported(Attribute attr);

        [ContractAnnotation("false => canbenull; true => notnull"), CanBeNull]
        protected TAttribute Get<TAttribute>(bool force = false) where TAttribute : Attribute
        {
            TAttribute attribute = _attributes.OfType<TAttribute>().FirstOrDefault();
            if (attribute == null && force)
                throw new InvalidOperationException($"Expected {typeof(TAttribute).Name}.");
            return attribute;
        }

        protected IEnumerable<TAttribute> GetAll<TAttribute>() where TAttribute : Attribute =>
            _attributes.OfType<TAttribute>();

        protected string Nullable(string value) => string.IsNullOrEmpty(value) ? null : value;

        public override string ToString() => $"{Name}{(IsPositional ? $"#{Index}" : "")}";
    }
}