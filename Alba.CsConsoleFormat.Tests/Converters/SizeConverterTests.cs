using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
#if HAS_INSTANCE_DESCRIPTOR
using System.ComponentModel.Design.Serialization;
#endif

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class SizeConverterTests
    {
        private readonly SizeConverter _converter = new SizeConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(Size)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(SizeConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(Size)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(SizeConverter)).Should().BeFalse();
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
            new Action(() => _converter.ConvertFrom("0")).Should().Throw<FormatException>();
            new Action(() => _converter.ConvertFrom("0 0 0")).Should().Throw<FormatException>();
            new Action(() => _converter.ConvertFrom("-1 1")).Should().Throw<FormatException>().WithMessage("*width*");
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("0 1").Should().Be(new Size(0, 1));
            _converter.ConvertFrom("2 3").Should().Be(new Size(2, 3));
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(new Size(), typeof(int))).Should().Throw<NotSupportedException>();
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
            _converter.ConvertToString(new Size(2, 3)).Should().Be("2 3");
            _converter.ConvertToString(new Size(-4, -5, false)).Should().Be("0 0");
        }

        #if HAS_INSTANCE_DESCRIPTOR
        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();
            _converter.ConvertTo(new Size(6, 2), typeof(InstanceDescriptor))
                .As<InstanceDescriptor>().Invoke()
                .Should().Be(new Size(6, 2));
        }
        #endif
    }
}