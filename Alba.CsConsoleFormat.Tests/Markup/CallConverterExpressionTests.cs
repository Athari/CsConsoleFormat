using System;
using System.Globalization;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests.Markup
{
    public class CallConverterExpressionTests
    {
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-us");
        private static readonly Foo Source = new Foo { Prefix = "A", Sub = new Foo { Prefix = "B" } };

        [Theory]
        [InlineData(nameof(Foo.InstanceConvert1), "B-1")]
        [InlineData(nameof(Foo.InstanceConvert2), "B-1-2")]
        [InlineData(nameof(Foo.InstanceConvert3), "B-1-2-en")]
        public void InstanceConvert(string methodName, string result)
        {
            var expr = new CallConverterExpression {
                Source = Source,
                Path = $"{nameof(Foo.Sub)}.{methodName}",
            };
            expr.GetValue(Source).As<ConverterDelegate>().Invoke(1, 2, Culture).Should().Be(result);
        }

        [Theory]
        [InlineData(nameof(Foo.StaticConvert1), "S-1")]
        [InlineData(nameof(Foo.StaticConvert2), "S-1-2")]
        [InlineData(nameof(Foo.StaticConvert3), "S-1-2-en")]
        public void StaticConvert(string methodName, string result)
        {
            var expr = new CallConverterExpression {
                Source = Source,
                Path = $"{nameof(Foo.Sub)}.{methodName}",
            };
            expr.GetValue(Source).As<ConverterDelegate>().Invoke(1, 2, Culture).Should().Be(result);
        }

        [Fact]
        public void InstanceConvertOverloaded()
        {
            var expr = new CallConverterExpression {
                Source = Source,
                Path = $"{nameof(Foo.Sub)}.{nameof(Foo.InstanceConvertOverloaded)}",
            };
            expr.GetValue(Source).As<ConverterDelegate>().Invoke(1, 2m, Culture).Should().Be("B-1-2m");
            expr.GetValue(Source).As<ConverterDelegate>().Invoke(1, 2f, Culture).Should().Be("B-1-2f");
        }

        [Fact]
        public void ConvertSource()
        {
            var expr = new CallConverterExpression {
                Source = new Func<object, object>(v => Source.InstanceConvert1((int)v)),
            };
            expr.GetValue(Source).As<ConverterDelegate>().Invoke(1, 2, Culture).Should().Be("A-1");
        }

        public class Foo
        {
            public Foo Sub { get; set; }
            public string Prefix { get; set; }

            public string InstanceConvert1(int value) => $"{Prefix}-{value}";
            public string InstanceConvert2(int value, int param) => $"{Prefix}-{value}-{param}";
            public string InstanceConvert3(int value, int param, CultureInfo culture) => $"{Prefix}-{value}-{param}-{culture.TwoLetterISOLanguageName}";

            public string InstanceConvertOverloaded(int value, decimal param) => $"{Prefix}-{value}-{param}m";
            public string InstanceConvertOverloaded(int value, float param) => $"{Prefix}-{value}-{param}f";

            public static string StaticConvert1(int value) => $"S-{value}";
            public static string StaticConvert2(int value, int param) => $"S-{value}-{param}";
            public static string StaticConvert3(int value, int param, CultureInfo culture) => $"S-{value}-{param}-{culture.TwoLetterISOLanguageName}";
        }
    }
}