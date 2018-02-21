namespace System
{
    namespace Diagnostics.Contracts
    {
        [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Delegate | AttributeTargets.Class | AttributeTargets.Parameter)]
        internal sealed class PureAttribute : Attribute
        {
            public PureAttribute()
            { }
        }
    }

    namespace Diagnostics.CodeAnalysis
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false)]
        internal sealed class ExcludeFromCodeCoverageAttribute : Attribute
        {
            public ExcludeFromCodeCoverageAttribute()
            { }
        }
    }
}