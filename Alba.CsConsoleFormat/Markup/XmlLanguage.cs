using System.ComponentModel;
using System.Globalization;

namespace Alba.CsConsoleFormat.Markup
{
    [TypeConverter (typeof(XmlLanguageConverter))]
    public class XmlLanguage
    {
        public string Name { get; set; }

        public XmlLanguage (string name)
        {
            Name = name;
        }

        public CultureInfo Culture => Name != null ? new CultureInfo(Name) : null;
    }
}