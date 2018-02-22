using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat.ColorfulConsole
{
    public sealed class FigletGradientConverter : SequenceTypeConverter<FigletGradient>
    {
        private static readonly ConsoleColorConverter ConsoleColorConverter = new ConsoleColorConverter();

        private static readonly Lazy<ConstructorInfo> FigletGradientConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(FigletGradient).GetConstructor(new[] { typeof(FigletGradientStopCollection) }));

        protected override FigletGradient FromString(string str)
        {
            List<List<string>> parts = str.Split(';').Select(sstop => SplitNumbers(sstop, 2).Select(s => s.Trim()).ToList()).ToList();
            var gradient = new FigletGradient();
            foreach (List<string> part in parts) {
                ConsoleColor? color;
                int offset;
                switch (part.Count) {
                    case 1:
                        color = GetColor(part[0]);
                        offset = FigletGradientStop.AutoOffset;
                        break;
                    case 2:
                        color = GetColor(part[0]);
                        offset = ParseInt(part[1]);
                        break;
                    default:
                        throw new FormatException($"Invalid {nameof(FigletGradient)} format: '{str}'.");
                }
                gradient.GradientStops.Add(new FigletGradientStop { Color = color, Offset = offset });
            }
            return gradient;
        }

        protected override ConstructorInfo InstanceConstructor => FigletGradientConstructor.Value;
        protected override object[] InstanceConstructorArgs(FigletGradient o) => new object[] { o.GradientStops };

        private static ConsoleColor? GetColor(string str) => (ConsoleColor?)ConsoleColorConverter.ConvertFromString(str);
    }
}