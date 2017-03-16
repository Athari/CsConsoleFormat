using System;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Point"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2" - <c>new Point(1, 2)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class PointConverter : SequenceTypeConverter<Point>
    {
        private static readonly Lazy<ConstructorInfo> PointConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Point).GetConstructor(new[] { typeof(int), typeof(int) }));

        protected override Point FromString(string str)
        {
            string[] parts = SplitNumbers(str, 2);
            if (parts.Length != 2)
                throw new FormatException($"Invalid Point format: '{str}'.");
            return new Point(ParseInt(parts[0]), ParseInt(parts[1]));
        }

        protected override ConstructorInfo InstanceConstructor => PointConstructor.Value;
        protected override object[] InstanceConstructorArgs(Point o) => new object[] { o.X, o.Y };
    }
}