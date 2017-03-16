using System;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class LineThicknessConverterTests
    {
        private readonly LineThicknessConverter _converter = new LineThicknessConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(int)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(InstanceDescriptor)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(LineThickness)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(LineThicknessConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(LineThickness)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(LineThicknessConverter)).Should().BeFalse();
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
            _converter.ConvertFrom("None").Should().Be(LineThickness.None);
            _converter.ConvertFrom("NonE").Should().Be(LineThickness.None);
            _converter.ConvertFrom("0").Should().Be(LineThickness.None);
            _converter.ConvertFrom("Single").Should().Be(LineThickness.Single);
            _converter.ConvertFrom("SinglE").Should().Be(LineThickness.Single);
            _converter.ConvertFrom("1").Should().Be(LineThickness.Single);
            _converter.ConvertFrom("Wide").Should().Be(LineThickness.Wide);
            _converter.ConvertFrom("WidE").Should().Be(LineThickness.Wide);
            _converter.ConvertFrom("2").Should().Be(LineThickness.Wide);

            _converter.ConvertFrom("None Single").Should().Be(new LineThickness(LineWidth.None, LineWidth.Single));
            _converter.ConvertFrom("Single 2").Should().Be(new LineThickness(LineWidth.Single, LineWidth.Wide));

            _converter.ConvertFrom("None Single Wide 0").Should().Be(new LineThickness(LineWidth.None, LineWidth.Single, LineWidth.Wide, LineWidth.None));
        }

        [Fact]
        public void ConvertFromNumber()
        {
            _converter.ConvertFrom(LineWidth.None).Should().Be(LineThickness.None);
            _converter.ConvertFrom(LineWidth.Single).Should().Be(LineThickness.Single);
            _converter.ConvertFrom(LineWidth.Wide).Should().Be(LineThickness.Wide);
            _converter.ConvertFrom(0).Should().Be(LineThickness.None);
            _converter.ConvertFrom(0m).Should().Be(LineThickness.None);
            _converter.ConvertFrom(1).Should().Be(LineThickness.Single);
            _converter.ConvertFrom(2L).Should().Be(LineThickness.Wide);
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(LineThickness.None, typeof(Guid))).ShouldThrow<NotSupportedException>();
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
            _converter.ConvertToString(new LineThickness(LineWidth.None, LineWidth.Single, LineWidth.Wide, LineWidth.None))
                .Should().Be("None Single Wide None");
        }

        [Fact]
        public void ConvertToInstanceDescriptor()
        {
            _converter.ConvertTo(LineThickness.Wide, typeof(InstanceDescriptor))
                .As<InstanceDescriptor>().Invoke()
                .Should().Be(LineThickness.Wide);
        }
    }
}