using System;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Vector"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2" - <c>new Vector(1, 2)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public sealed class VectorConverter : SequenceTypeConverter<Vector>
    {
        private static readonly Lazy<ConstructorInfo> VectorConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Vector).GetConstructor(new[] { typeof(int), typeof(int) }));

        protected override Vector FromString(string str)
        {
            string[] parts = SplitNumbers(str, 2);
            if (parts.Length != 2)
                throw new FormatException($"Invalid Vector format: '{str}'.");
            return new Vector(ParseInt(parts[0]), ParseInt(parts[1]));
        }

        protected override ConstructorInfo InstanceConstructor => VectorConstructor.Value;
        protected override object[] InstanceConstructorArgs(Vector o) => new object[] { o.X, o.Y };
    }
}