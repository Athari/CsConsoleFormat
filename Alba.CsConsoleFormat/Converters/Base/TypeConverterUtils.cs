using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    internal static class TypeConverterUtils
    {
        public const string Asterisk = "*";
        public const string Auto = nameof(Auto);
        public const string AUTO = nameof(AUTO);
        public const string Inherit = nameof(Inherit);
        public const string INHERIT = nameof(INHERIT);

        [Pure]
        public static bool IsTypeStringOrNumeric([CanBeNull] Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            return type != null && (code == TypeCode.String || TypeCode.Int16 <= code && code <= TypeCode.Decimal && !type.IsEnum);
        }

        [Pure]
        public static bool IsTypeNumeric([CanBeNull] Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            return type != null && TypeCode.Int16 <= code && code <= TypeCode.Decimal && !type.IsEnum;
        }

        [Pure]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "API consistency.")]
        public static bool IsTypeStringOrNumeric([CanBeNull] this object value) =>
            value != null && IsTypeStringOrNumeric(value.GetType());

        [Pure]
        public static bool IsTypeNumeric([CanBeNull] this object value) =>
            value != null && IsTypeNumeric(value.GetType());

        [Pure]
        public static T StringToEnum<T>([CanBeNull] string str)
            where T : struct =>
            Enum.TryParse(str, true, out T value) ? value : throw new FormatException($"'{str}' is not a valid {typeof(T).Name} value.");

        [Pure]
        public static T NumberToEnum<T>([NotNull] object number) =>
            (T)(object)Convert.ToInt32(number ?? throw new ArgumentNullException(nameof(number)), CultureInfo.InvariantCulture);

        [Pure]
        public static int ParseInt([NotNull] string str) =>
            int.Parse(str ?? throw new ArgumentNullException(nameof(str)), CultureInfo.InvariantCulture);

        [Pure]
        public static int ToInt([NotNull] object obj) =>
            Convert.ToInt32(obj ?? throw new ArgumentNullException(nameof(obj)), CultureInfo.InvariantCulture);

        [Pure]
        public static string[] SplitNumbers([NotNull] string str, int count) =>
            (str ?? throw new ArgumentNullException(nameof(str))).Split(new[] { ' ', ',' }, count, StringSplitOptions.RemoveEmptyEntries);
    }
}