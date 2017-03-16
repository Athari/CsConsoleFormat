using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Alba.CsConsoleFormat.Markup
{
    [TypeConverter(typeof(XmlLanguageConverter))]
    public class XmlLanguage
    {
        private CultureInfo _culture;

        [CanBeNull]
        public string Name { get; set; }

        public XmlLanguage(string name)
        {
            Name = name;
        }

        public XmlLanguage([NotNull] CultureInfo culture)
        {
            _culture = culture ?? throw new ArgumentNullException(nameof(culture));
            Name = culture.Name;
        }

        [CanBeNull]
        public CultureInfo Culture => Name == null ? null : (_culture ?? (_culture = new CultureInfo(Name)));

        public override string ToString() => Name ?? _culture?.Name ?? "";
        public override bool Equals(object obj) => obj is XmlLanguage lng && lng.Name == Name;
        public override int GetHashCode() => Name?.GetHashCode() ?? 0;
    }
}