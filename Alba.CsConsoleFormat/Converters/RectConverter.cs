using System;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Rect"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2 3 4" - <c>new Rect(1, 2, 3, 4)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    public class RectConverter : SequenceTypeConverter<Rect>
    {
        private static readonly Lazy<ConstructorInfo> RectConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Rect).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }));

        protected override Rect FromString(string str)
        {
            string[] parts = SplitNumbers(str, 4);
            if (parts.Length != 4)
                throw new FormatException($"Invalid Rect format: '{str}'.");
            try {
                return new Rect(ParseInt(parts[0]), ParseInt(parts[1]), ParseInt(parts[2]), ParseInt(parts[3]));
            }
            catch (ArgumentException ex) {
                throw new FormatException($"Invalid Rect format: '{0}'. {ex.Message}", ex);
            }
        }

        protected override ConstructorInfo InstanceConstructor => RectConstructor.Value;
        protected override object[] InstanceConstructorArgs(Rect o) => new object[] { o.X, o.Y, o.Width, o.Height, false };
    }
}