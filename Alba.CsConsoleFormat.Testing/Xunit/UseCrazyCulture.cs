using System;
using System.Globalization;

namespace Alba.CsConsoleFormat.Testing.Xunit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseCrazyCultureAttribute : UseCultureAttribute
    {
        public UseCrazyCultureAttribute() : base("tr")
        {}

        protected override CultureInfo OverrideCulture(CultureInfo culture)
        {
            culture.NumberFormat.NegativeSign = "#";
            culture.NumberFormat.NumberGroupSeparator = "@";
            return culture;
        }
    }
}