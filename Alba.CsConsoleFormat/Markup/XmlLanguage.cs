using System.ComponentModel;
using System.Globalization;

namespace Alba.CsConsoleFormat.Markup
{
    [TypeConverter (typeof(XmlLanguageConverter))]
    public class XmlLanguage
    {
        public string Name { get; set; }

        public CultureInfo Culture
        {
            get { return Name != null ? new CultureInfo(Name) : null; }
        }

        public XmlLanguage (string name)
        {
            Name = name;
        }
    }
}