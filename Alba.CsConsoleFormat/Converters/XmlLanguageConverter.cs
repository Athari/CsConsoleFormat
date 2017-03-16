using System;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="XmlLanguage"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"en-us" - <c>new XmlLanguage("en-us")</c></item>
    /// </list> 
    /// </summary>
    public class XmlLanguageConverter : SequenceTypeConverter<XmlLanguage>
    {
        private static readonly Lazy<ConstructorInfo> XmlLanguageConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(XmlLanguage).GetConstructor(new[] { typeof(string) }));

        protected override XmlLanguage FromString(string str) => new XmlLanguage(str);
        protected override ConstructorInfo InstanceConstructor => XmlLanguageConstructor.Value;
        protected override object[] InstanceConstructorArgs(XmlLanguage o) => new object[] { o.Name };
    }
}