using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
#if HAS_INSTANCE_DESCRIPTOR
using System.ComponentModel.Design.Serialization;
#endif

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class LengthConverterTests
    {
        private readonly LengthConverter _converter = new LengthConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(int)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(long)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(LengthConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(LengthConverter)).Should().BeFalse();
        }

        [Fact]
        public void ConvertFromInvalidSource()
        {
            new Action(() => _converter.ConvertFrom(null)).Should().Throw<NotSupportedException>().WithMessage("*null*");
            new Action(() => _converter.ConvertFrom(new object())).Should().Throw<NotSupportedException>().WithMessage($"*{typeof(object)}*");
        }

        [Fact]
        public void ConvertFromInvalidSourceFormat()
        {
            new Action(() => _converter.ConvertFrom("&")).Should().Throw<FormatException>();
            new Action(() => _converter.ConvertFrom("0L")).Should().Throw<FormatException>();
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("auto").Should().Be(null);
            _converter.ConvertFrom("AutO").Should().Be(null);
            _converter.ConvertFrom("3").Should().Be(3);
        }

        [Fact]
        public void ConvertFromNumber()
        {
            _converter.ConvertFrom(3).Should().Be(3);
            _converter.ConvertFrom(4L).Should().Be(4);
            _converter.ConvertFrom(5m).Should().Be(5);
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(1, typeof(Guid))).Should().Throw<NotSupportedException>();
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToInvalidSource()
        {
            new Action(() => _converter.ConvertTo("1337", typeof(string))).Should().Throw<NotSupportedException>();
            new Action(() => _converter.ConvertTo(4m, typeof(string))).Should().Throw<NotSupportedException>();
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToString()
        {
            _converter.ConvertToString(4).Should().Be("4");
            _converter.ConvertToString(null).Should().Be("Auto");
        }

        #if HAS_INSTANCE_DESCRIPTOR
        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeFalse();
        }
        #endif
    }
}