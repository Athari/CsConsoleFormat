using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using Xunit.Sdk;

namespace Alba.CsConsoleFormat.Testing.Xunit
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly Lazy<CultureInfo> _culture;
        private readonly Lazy<CultureInfo> _uiCulture;
        private CultureInfo _originalCulture;
        private CultureInfo _originalUICulture;

        public UseCultureAttribute(string culture) : this(culture, culture)
        { }

        public UseCultureAttribute(string culture, string uiCulture)
        {
            _culture = new Lazy<CultureInfo>(() => OverrideCulture(new CultureInfo(culture)));
            _uiCulture = new Lazy<CultureInfo>(() => OverrideCulture(new CultureInfo(uiCulture)));
        }

        public CultureInfo Culture => _culture.Value;
        public CultureInfo UICulture => _uiCulture.Value;

        protected virtual CultureInfo OverrideCulture(CultureInfo culture) => culture;

        public override void Before(MethodInfo methodUnderTest)
        {
          #if NET_FULL
            _originalCulture = Thread.CurrentThread.CurrentCulture;
            _originalUICulture = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = UICulture;
          #elif NET_STANDARD
            _originalCulture = CultureInfo.CurrentCulture;
            _originalUICulture = CultureInfo.CurrentUICulture;

            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = UICulture;
          #endif
        }

        public override void After(MethodInfo methodUnderTest)
        {
          #if NET_FULL
            Thread.CurrentThread.CurrentCulture = _originalCulture;
            Thread.CurrentThread.CurrentUICulture = _originalUICulture;
          #elif NET_STANDARD
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUICulture;
          #endif
        }
    }
}