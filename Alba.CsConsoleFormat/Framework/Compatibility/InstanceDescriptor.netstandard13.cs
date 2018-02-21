using System.Collections;
using System.Reflection;

// ReSharper disable All
namespace System.ComponentModel.Design.Serialization
{
    /// <summary>Stub InstanceDescriptor.</summary>
    /// <remarks>Easier to create a fake than to branch in #ifs.</remarks>
    internal sealed class InstanceDescriptor
    {
        public InstanceDescriptor(MemberInfo member, ICollection arguments) : this(member, arguments, true)
        { }

        public InstanceDescriptor(MemberInfo member, ICollection arguments, bool isComplete)
        { }

        public ICollection Arguments { get; }
        public bool IsComplete { get; }
        public MemberInfo MemberInfo { get; }
        public object Invoke() => null;
    }
}