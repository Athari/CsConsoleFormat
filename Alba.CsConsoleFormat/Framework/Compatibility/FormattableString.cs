using System.Globalization;

// ReSharper disable CheckNamespace
namespace System
{
    internal abstract class FormattableString : IFormattable
    {
        public abstract string Format { get; }
        public abstract int ArgumentCount { get; }
        public abstract object[] GetArguments ();
        public abstract object GetArgument (int index);

        public abstract string ToString (IFormatProvider provider);
        string IFormattable.ToString (string _, IFormatProvider provider) => ToString(provider);
        public override string ToString () => ToString(CultureInfo.CurrentCulture);

        public static string Invariant (FormattableString formattable) => formattable.ToString(CultureInfo.InvariantCulture);
    }

    namespace Runtime.CompilerServices
    {
        internal static class FormattableStringFactory
        {
            public static FormattableString Create (string format, params object[] arguments) => new ConcreteFormattableString(format, arguments);

            private class ConcreteFormattableString : FormattableString
            {
                private readonly string _format;
                private readonly object[] _arguments;

                internal ConcreteFormattableString (string format, object[] arguments)
                {
                    if (format == null)
                        throw new ArgumentNullException(nameof(format));
                    if (arguments == null)
                        throw new ArgumentNullException(nameof(arguments));
                    _format = format;
                    _arguments = arguments;
                }

                public override string Format => _format;
                public override int ArgumentCount => _arguments.Length;
                public override object[] GetArguments () => _arguments;
                public override object GetArgument (int index) => _arguments[index];

                public override string ToString (IFormatProvider provider) => string.Format(provider, _format, _arguments);
            }
        }
    }
}