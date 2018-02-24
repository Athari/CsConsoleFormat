using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
#if HAS_INSTANCE_DESCRIPTOR
using System.ComponentModel.Design.Serialization;
#endif

namespace Alba.CsConsoleFormat.Tests
{
    public sealed class ConsoleColorConverterTests
    {
        private readonly ConsoleColorConverter _converter = new ConsoleColorConverter();

        [Fact]
        public void CanConvertFrom()
        {
            _converter.CanConvertFrom(null, typeof(int)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(decimal)).Should().BeTrue();
            _converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertFrom(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(ConsoleColor)).Should().BeFalse();
            _converter.CanConvertFrom(null, typeof(ConsoleColorConverter)).Should().BeFalse();
        }

        [Fact]
        public void CanConvertTo()
        {
            _converter.CanConvertTo(null, typeof(int)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(decimal)).Should().BeTrue();
            _converter.CanConvertTo(null, typeof(string)).Should().BeTrue();

            _converter.CanConvertTo(null, typeof(void)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(object)).Should().BeFalse();
            _converter.CanConvertTo(null, typeof(ConsoleColor)).Should().BeFalse();
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
            new Action(() => _converter.ConvertFrom("1L")).Should().Throw<FormatException>();
            new Action(() => _converter.ConvertFrom("pink")).Should().Throw<FormatException>();
        }

        [Fact]
        public void ConvertFromString()
        {
            _converter.ConvertFrom("inherit").Should().Be(null);
            _converter.ConvertFrom("InheriT").Should().Be(null);

            _converter.ConvertFrom("0").Should().Be(ConsoleColor.Black);
            _converter.ConvertFrom("+1").Should().Be(ConsoleColor.DarkBlue);

            _converter.ConvertFrom("darkred").Should().Be(ConsoleColor.DarkRed);
            _converter.ConvertFrom("DarkRed").Should().Be(ConsoleColor.DarkRed);
            _converter.ConvertFrom("DARKRED").Should().Be(ConsoleColor.DarkRed);
        }

        [Fact]
        public void ConvertFromNumber()
        {
            _converter.ConvertFrom(0).Should().Be(ConsoleColor.Black);
            _converter.ConvertFrom(1m).Should().Be(ConsoleColor.DarkBlue);
            _converter.ConvertFrom(2).Should().Be(ConsoleColor.DarkGreen);
            _converter.ConvertFrom(3L).Should().Be(ConsoleColor.DarkCyan);
        }

        [Fact]
        public void ConvertToInvalidDestination()
        {
            new Action(() => _converter.ConvertTo(ConsoleColor.Yellow, typeof(Guid))).Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ConvertToInvalidSource()
        {
            new Action(() => _converter.ConvertTo(1337, typeof(string))).Should().Throw<NotSupportedException>();
            new Action(() => _converter.ConvertTo(1337, typeof(int))).Should().Throw<NotSupportedException>();
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToString()
        {
            _converter.ConvertToString(null).Should().Be("Inherit");
            _converter.ConvertToString(ConsoleColor.DarkYellow).Should().Be("DarkYellow");
        }

        [Fact, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ConvertToNumber()
        {
            _converter.ConvertTo(null, typeof(float)).Should().Be(null);
            _converter.ConvertTo(ConsoleColor.Gray, typeof(int)).Should().Be((int)ConsoleColor.Gray);
            _converter.ConvertTo(ConsoleColor.White, typeof(decimal)).Should().Be((decimal)ConsoleColor.White);
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