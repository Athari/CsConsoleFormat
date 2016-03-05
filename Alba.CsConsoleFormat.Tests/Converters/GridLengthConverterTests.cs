using System;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class GridLengthConverterTests
    {
        private readonly GridLengthConverter _converter = new GridLengthConverter();

        [Fact]
        public void ConvertFromInvalidSource ()
        {
            new Action(() => _converter.ConvertFrom(null)).ShouldThrow<NotSupportedException>();
            new Action(() => _converter.ConvertFrom(new object())).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertFromInvalidSourceFormat ()
        {
            new Action(() => _converter.ConvertFrom("&")).ShouldThrow<FormatException>();
        }

        [Fact]
        public void ConvertFromString ()
        {
            _converter.ConvertFrom("Auto").Should().Be(GridLength.Auto);
            _converter.ConvertFrom("AutO").Should().Be(GridLength.Auto);
            _converter.ConvertFrom("1").Should().Be(GridLength.Char(1));
            _converter.ConvertFrom("5").Should().Be(GridLength.Char(5));
            _converter.ConvertFrom("5").Should().Be(GridLength.Char(5));
            _converter.ConvertFrom("1*").Should().Be(GridLength.Star(1));
            _converter.ConvertFrom("5*").Should().Be(GridLength.Star(5));
            _converter.ConvertFrom("*").Should().Be(GridLength.Star(1));
        }

        [Fact]
        public void ConvertFromNumber ()
        {
            _converter.ConvertFrom(2).Should().Be(GridLength.Char(2));
            _converter.ConvertFrom(2m).Should().Be(GridLength.Char(2));
            _converter.ConvertFrom(2L).Should().Be(GridLength.Char(2));
        }

        [Fact]
        public void ConvertToInvalidDestination ()
        {
            new Action(() => _converter.ConvertTo(GridLength.Auto, typeof(Guid))).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertToInvalidSource ()
        {
            new Action(() => _converter.ConvertTo("Auto", typeof(string))).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertToString ()
        {
            _converter.ConvertToString(GridLength.Auto).Should().Be("Auto");
            _converter.ConvertToString(GridLength.Char(1)).Should().Be("1");
            _converter.ConvertToString(GridLength.Char(5)).Should().Be("5");
            _converter.ConvertToString(GridLength.Star(1)).Should().Be("*");
            _converter.ConvertToString(GridLength.Star(5)).Should().Be("5*");
        }
    }
}