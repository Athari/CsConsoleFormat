using System;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class RectConverterTests
    {
        private readonly RectConverter _converter = new RectConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(Rect)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(RectConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(Rect)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(RectConverter)).Should().BeFalse();
        }

        [Fact]
        public void ConvertFromInvalidSource()
        {
            new Action(() => _converter.ConvertFrom(null)).ShouldThrow<NotSupportedException>().WithMessage("*null*");
            new Action(() => _converter.ConvertFrom(new object())).ShouldThrow<NotSupportedException>().WithMessage($"*{typeof(object)}*");
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
            _converter.ConvertFrom("0 1, 2 3").Should().Be(new Rect(0, 1, 2, 3));
            _converter.ConvertFrom("2, 3 5, 1").Should().Be(new Rect(2, 3, 5, 1));
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(new Rect(), typeof(int))).ShouldThrow<NotSupportedException>();
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToInvalidSource()
        {
            new Action(() => _converter.ConvertTo(1337, typeof(string))).ShouldThrow<NotSupportedException>();
            new Action(() => _converter.ConvertTo(null, typeof(string))).ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ConvertToString()
        {
            _converter.ConvertToString(new Rect(2, 3, 1, 1)).Should().Be("2 3 1 1");
            _converter.ConvertToString(new Rect(-4, -5, 2, 3)).Should().Be("-4 -5 2 3");
        }

        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.ConvertTo(new Rect(1, 2, 7, 6), typeof(InstanceDescriptor))
                .As<InstanceDescriptor>().Invoke()
                .Should().Be(new Rect(1, 2, 7, 6));
        }
    }
}