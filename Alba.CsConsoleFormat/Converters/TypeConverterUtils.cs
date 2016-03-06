using System;
using System.Globalization;

namespace Alba.CsConsoleFormat
{
    internal static class TypeConverterUtils
    {
        public const string Asterisk = "*";
        public const string Auto = nameof(Auto);
        public const string AUTO = nameof(AUTO);
        public const string Inherit = nameof(Inherit);
        public const string INHERIT = nameof(INHERIT);

        public static bool IsTypeStringOrNumeric(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            return code == TypeCode.String || TypeCode.Int16 <= code && code <= TypeCode.Decimal;
        }

        public static bool IsTypeNumeric(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            return TypeCode.Int16 <= code && code <= TypeCode.Decimal;
        }

        public static T ParseEnum<T>(string str)
            where T : struct
        {
            T value;
            if (Enum.TryParse(str, true, out value))
                return value;
            else
                throw new FormatException($"'{str}' is not a valid {typeof(T).Name} value.");
        }

        public static T ToEnum<T>(object obj) => (T)(object)Convert.ToInt32(obj, CultureInfo.InvariantCulture);
        public static int ParseInt(string str) => int.Parse(str, CultureInfo.InvariantCulture);
        public static int ToInt(object obj) => Convert.ToInt32(obj, CultureInfo.InvariantCulture);

        public static string[] SplitNumbers(string str, int count) => str.Split(new[] { ' ', ',' }, count, StringSplitOptions.RemoveEmptyEntries);
    }
}