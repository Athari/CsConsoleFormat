using System;
using FluentAssertions;
using Xunit;
#if HAS_INSTANCE_DESCRIPTOR
using System.ComponentModel.Design.Serialization;
#endif

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class GridLengthConverterTests
    {
        private readonly GridLengthConverter _converter = new GridLengthConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(int)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(ConsoleColorConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(ConsoleColorConverter)).Should().BeFalse();
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
            new Action(() => _converter.ConvertFrom("**")).Should().Throw<FormatException>();
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("Auto").Should().Be(GridLength.Auto);
            _converter.ConvertFrom("AutO").Should().Be(GridLength.Auto);
            _converter.ConvertFrom("1").Should().Be(GridLength.Char(1));
            _converter.ConvertFrom("5").Should().Be(GridLength.Char(5));
            _converter.ConvertFrom("5").Should().Be(GridLength.Char(5));
            _converter.ConvertFrom("1*").Should().Be(GridLength.Star(1));
            _converter.ConvertFrom("5*").Should().Be(GridLength.Star(5));
            _converter.ConvertFrom("5 *").Should().Be(GridLength.Star(5));
            _converter.ConvertFrom("*").Should().Be(GridLength.Star(1));
        }

        [Fact]
        public void ConvertFromNumber()
        {
            _converter.ConvertFrom(2).Should().Be(GridLength.Char(2));
            _converter.ConvertFrom(2m).Should().Be(GridLength.Char(2));
            _converter.ConvertFrom(2L).Should().Be(GridLength.Char(2));
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(GridLength.Auto, typeof(Guid))).Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ConvertToInvalidSource()
        {
            new Action(() => _converter.ConvertTo("Auto", typeof(string))).Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ConvertToString()
        {
            _converter.ConvertToString(GridLength.Auto).Should().Be("Auto");
            _converter.ConvertToString(GridLength.Char(1)).Should().Be("1");
            _converter.ConvertToString(GridLength.Char(5)).Should().Be("5");
            _converter.ConvertToString(GridLength.Star(1)).Should().Be("*");
            _converter.ConvertToString(GridLength.Star(5)).Should().Be("5*");
        }

        #if HAS_INSTANCE_DESCRIPTOR
        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.ConvertTo(GridLength.Star(3), typeof(InstanceDescriptor))
                .As<InstanceDescriptor>().Invoke()
                .Should().Be(GridLength.Star(3));
        }
        #endif
    }
}