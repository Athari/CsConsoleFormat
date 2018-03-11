using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class XmlLanguageConverter : SequenceTypeConverter<XmlLanguage>
    {
        private static readonly Lazy<ConstructorInfo> XmlLanguageConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(XmlLanguage).GetConstructor(new[] { typeof(string) }));

        protected override XmlLanguage FromString(string str) => new XmlLanguage(str);
        protected override ConstructorInfo InstanceConstructor => XmlLanguageConstructor.Value;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Caller guarantees non-null value.")]
        protected override object[] InstanceConstructorArgs(XmlLanguage o) => new object[] { o.Name };
    }
}