using System;
using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using Xunit;
#if HAS_INSTANCE_DESCRIPTOR
using System.ComponentModel.Design.Serialization;
#endif

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class XmlLanguageConverterTests
    {
        private readonly XmlLanguageConverter _converter = new XmlLanguageConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(XmlLanguage)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(XmlLanguageConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(XmlLanguage)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(XmlLanguageConverter)).Should().BeFalse();
        }

        [Fact]
        public void ConvertFromInvalidSource()
        {
            new Action(() => _converter.ConvertFrom(null)).Should().Throw<NotSupportedException>().WithMessage("*null*");
            new Action(() => _converter.ConvertFrom(new object())).Should().Throw<NotSupportedException>().WithMessage($"*{typeof(object)}*");
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("en-us").Should().Be(new XmlLanguage("en-us"));
            _converter.ConvertFrom("ru-ru").Should().Be(new XmlLanguage("ru-ru"));
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(new XmlLanguage("en"), typeof(int))).Should().Throw<NotSupportedException>();
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToInvalidSource()
        {
            new Action(() => _converter.ConvertTo(1337, typeof(string))).Should().Throw<NotSupportedException>();
            new Action(() => _converter.ConvertTo(null, typeof(string))).Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ConvertToString()
        {
            _converter.ConvertToString(new XmlLanguage("en-us")).Should().Be("en-us");
            _converter.ConvertToString(new XmlLanguage("ru")).Should().Be("ru");
        }

        #if HAS_INSTANCE_DESCRIPTOR
        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.ConvertTo(new XmlLanguage("en-us"), typeof(InstanceDescriptor))
                .As<InstanceDescriptor>().Invoke()
                .Should().Be(new XmlLanguage("en-us"));
        }
        #endif
    }
}