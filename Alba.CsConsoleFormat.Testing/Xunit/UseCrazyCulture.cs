using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Testing.Xunit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class UseCrazyCultureAttribute : UseCultureAttribute
    {
        public UseCrazyCultureAttribute() : base("tr")
        { }

        protected override CultureInfo OverrideCulture([NotNull] CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            culture.NumberFormat.NegativeSign = "#";
            culture.NumberFormat.NumberGroupSeparator = "@";
            return culture;
        }
    }
}