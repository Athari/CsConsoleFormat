using System;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class ThicknessConverterTests
    {
        private readonly ThicknessConverter _converter = new ThicknessConverter();

        [Fact]
        public void ConvertFromInvalidSource()
        {
            new Action(() => _converter.ConvertFrom(null)).ShouldThrow<NotSupportedException>();
            new Action(() => _converter.ConvertFrom(new object())).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertFromInvalidSourceFormat()
        {
            new Action(() => _converter.ConvertFrom("&")).ShouldThrow<FormatException>();
            new Action(() => _converter.ConvertFrom("0 0 0")).ShouldThrow<FormatException>();
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("0").Should().Be(new Thickness(0));
            _converter.ConvertFrom("2").Should().Be(new Thickness(2));

            _converter.ConvertFrom("0 1").Should().Be(new Thickness(0, 1));
            _converter.ConvertFrom("2 3").Should().Be(new Thickness(2, 3));

            _converter.ConvertFrom("1 2 3 4").Should().Be(new Thickness(1, 2, 3, 4));
        }

        [Fact]
        public void ConvertFromNumber()
        {
            _converter.ConvertFrom(0).Should().Be(new Thickness(0));
            _converter.ConvertFrom(0m).Should().Be(new Thickness(0));
            _converter.ConvertFrom(1).Should().Be(new Thickness(1));
            _converter.ConvertFrom(2L).Should().Be(new Thickness(2));
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(new Thickness(), typeof(Guid))).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertToString()
        {
            _converter.ConvertToString(new Thickness(1, 2, 3, 4)).Should().Be("1 2 3 4");
        }
    }
}