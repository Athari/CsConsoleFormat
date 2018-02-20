using System.Globalization;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBeProtected.Global
namespace System
{
    [PublicAPI]
    internal abstract class FormattableString : IFormattable
    {
        public abstract string Format { get; }
        public abstract int ArgumentCount { get; }
        public abstract object[] GetArguments();
        public abstract object GetArgument(int index);

        public abstract string ToString(IFormatProvider provider);
        string IFormattable.ToString(string _, IFormatProvider provider) => ToString(provider);
        public override string ToString() => ToString(CultureInfo.CurrentCulture);

        public static string Invariant([NotNull] FormattableString formattable) => formattable.ToString(CultureInfo.InvariantCulture);
    }

    namespace Runtime.CompilerServices
    {
        [PublicAPI]
        internal static class FormattableStringFactory
        {
            public static FormattableString Create([NotNull] string format, [NotNull] params object[] arguments) => new ConcreteFormattableString(format, arguments);

            private sealed class ConcreteFormattableString : FormattableString
            {
                private readonly string _format;
                private readonly object[] _arguments;

                internal ConcreteFormattableString([NotNull] string format, [NotNull] object[] arguments)
                {
                    _format = format ?? throw new ArgumentNullException(nameof(format));
                    _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
                }

                public override string Format => _format;
                public override int ArgumentCount => _arguments.Length;
                public override object[] GetArguments() => _arguments;
                public override object GetArgument(int index) => _arguments[index];

                public override string ToString(IFormatProvider provider) => string.Format(provider, _format, _arguments);
            }
        }
    }
}