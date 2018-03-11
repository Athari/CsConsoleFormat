﻿using System;
using System.ComponentModel;
using System.Reflection;
using static Alba.CsConsoleFormat.TypeConverterUtils;

namespace Alba.CsConsoleFormat
{
    /// <summary>
    /// Converts <see cref="Size"/> to and from <see cref="string"/>:
    /// <list type="bullet">
    /// <item>"1 2" - <c>new Size(1, 2)</c></item>
    /// </list> 
    /// Separator can be " " or ",".
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class SizeConverter : SequenceTypeConverter<Size>
    {
        private static readonly Lazy<ConstructorInfo> SizeConstructor = new Lazy<ConstructorInfo>(() =>
            typeof(Size).GetConstructor(new[] { typeof(int), typeof(int), typeof(bool) }));

        protected override Size FromString(string str)
        {
            string[] parts = SplitNumbers(str, 2);
            if (parts.Length != 2)
                throw new FormatException($"Invalid Size format: '{0}'.");
            try {
                return new Size(ParseInt(parts[0]), ParseInt(parts[1]));
            }
            catch (ArgumentException ex) {
                throw new FormatException($"Invalid Size format: '{0}'. {ex.Message}", ex);
            }
        }

        protected override ConstructorInfo InstanceConstructor => SizeConstructor.Value;
        protected override object[] InstanceConstructorArgs(Size o) => new object[] { o.Width, o.Height, false };
    }
}