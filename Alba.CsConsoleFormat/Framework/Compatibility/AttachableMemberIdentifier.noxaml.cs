using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Portable.Xaml
{
    [SuppressMessage("ReSharper", "MergeConditionalExpression", Justification = "Conventional pattern.")]
    internal class AttachableMemberIdentifier : IEquatable<AttachableMemberIdentifier>
    {
        public AttachableMemberIdentifier(Type declaringType, string memberName)
        {
            DeclaringType = declaringType;
            MemberName = memberName;
        }

        public Type DeclaringType { get; }
        public string MemberName { get; }

        public override bool Equals(object obj) =>
            Equals(obj as AttachableMemberIdentifier);

        public bool Equals([CanBeNull] AttachableMemberIdentifier other) =>
            !ReferenceEquals(other, null) && DeclaringType == other.DeclaringType && MemberName == other.MemberName;

        public static bool operator ==([CanBeNull] AttachableMemberIdentifier left, [CanBeNull] AttachableMemberIdentifier right) =>
            ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=([CanBeNull] AttachableMemberIdentifier left, [CanBeNull] AttachableMemberIdentifier right) =>
            !(left == right);

        public override int GetHashCode() =>
            (DeclaringType?.GetHashCode() ?? 0) << 5 + (MemberName?.GetHashCode() ?? 0);

        public override string ToString() =>
            DeclaringType != null ? $"{DeclaringType.FullName}.{MemberName}" : MemberName;
    }
}