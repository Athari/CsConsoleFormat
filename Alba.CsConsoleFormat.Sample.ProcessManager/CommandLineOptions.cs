using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    public static class CommandLineOptions
    {
        public static IEnumerable<BaseOptionAttribute> GetOptions (Type optionsType) =>
            optionsType.GetProperties()
                .Select(p => p.GetCustomAttribute<BaseOptionAttribute>())
                .Where(a => a != null);

        public static ILookup<BaseOptionAttribute, BaseOptionAttribute> GetAllOptions (Type optionsType) =>
            GetOptions(optionsType)
                .SelectMany(
                    verb => GetOptions(GetVerbTypeByName(optionsType, verb.LongName)),
                    (verb, option) => new { verb, option })
                .ToLookup(
                    pair => pair.verb, pair => pair.option,
                    new KeyEqualityComparer<BaseOptionAttribute, string>(v => v.LongName));

        public static Type GetVerbTypeByName (Type optionsType, string verbName) =>
            optionsType.GetProperties()
                .Select(property => new { property, attribute = property.GetCustomAttribute<VerbOptionAttribute>() })
                .FirstOrDefault(o => o.attribute?.LongName == verbName)
                ?.property.PropertyType;

        private sealed class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
        {
            private readonly Func<T, TKey> _getKey;

            public KeyEqualityComparer (Func<T, TKey> getKey)
            {
                _getKey = getKey;
            }

            public bool Equals (T x, T y) => Equals(_getKey(x), _getKey(y));
            public int GetHashCode (T obj) => _getKey(obj).GetHashCode();
        }
    }
}