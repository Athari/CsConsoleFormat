using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

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
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            _culture = culture;
            Name = culture.Name;
        }

        [CanBeNull]
        public CultureInfo Culture => Name == null ? null : (_culture ?? (_culture = new CultureInfo(Name)));
    }
}