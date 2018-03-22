using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
#if NET_40 || NET_STANDARD_15
using Alba.CsConsoleFormat.CommandLineParser.Framework;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser
{
    internal static partial class ClpUtils
    {
        private static ClpFeatures _features;

        public static ClpFeatures Features
        {
            get
            {
                if (_features == ClpFeatures.None) {
                    Type parserType = Type.GetType("CommandLine.Parser, CommandLine", true);
                    if (parserType == null)
                        _features = ClpFeatures.VersionUnknown;
                    else {
                        Version version = parserType.GetAssembly().GetName().Version;
                        // HACK This stupid library has random version attributes, no idea WTF is going on.
                        if (version >= new Version(2, 0, 0))
                            _features = ClpFeatures.Version22 | ClpFeatures.Examples | ClpFeatures.BuiltInHelpVersion;
                        else if (version >= new Version(1, 9, 0))
                            _features = ClpFeatures.Version19;
                        else
                            _features = ClpFeatures.None;
                    }
                }
                return _features;
            }
        }

        public static bool IsVersion19 => (Features & ClpFeatures.Version19) != 0;

        public static bool IsVersion22 => (Features & ClpFeatures.Version22) != 0;

        public static string GetAssemblyUsageText(Assembly assembly)
        {
            string result = null;
          #if CLP_19
            if (IsVersion19)
                result = GetAssemblyUsageText19(assembly);
          #endif
          #if CLP_22
            if (IsVersion22)
                result = GetAssemblyUsageText22(assembly);
          #endif
            return result?.Trim() ?? "";
        }

        public static string GetAssemblyLicenseText(Assembly assembly)
        {
            string result = null;
          #if CLP_19
            if (IsVersion19)
                result = GetAssemblyLicenseText19(assembly);
          #endif
          #if CLP_22
            if (IsVersion22)
                result = GetAssemblyLicenseText22(assembly);
          #endif
            return result?.Trim() ?? "";
        }

        public static IEnumerable<OptionInfo> GetOptionsFromOptionsRoots(List<Type> optionsRoots)
        {
            var roots = GetTypeInfos(optionsRoots);
          #if CLP_22
            if (roots.Count > 1 || roots.SelectMany(GetAttributes).Any(a => a.GetType().FullName == VerbAttributeTypeName22))
                return roots.Select(CreateOptionVerb);
          #endif
            if (roots.Count == 1) {
                var props = roots.Single().GetProperties();
          #if CLP_19
                if (props.SelectMany(GetAttributes).Any(a => a.GetType().FullName == VerbOptionAttributeTypeName19))
                    return props
                        .Where(p => p.GetCustomAttributes<Attribute>().Any(a => a.GetType().FullName == VerbOptionAttributeTypeName19))
                        .Select(CreateOptionVerb);
          #endif
          #if CLP_19 || CLP_22
                return props.Select(CreateOption);
          #endif
            }
            throw UnsupportedVersion();

            OptionInfo CreateOptionVerb(MemberInfo root) => OptionInfo.FromMember(root, GetAttributes(root), isVerb: true)
                .WithSubOptions(GetVerbOptions(root).Select(CreateOption));

            OptionInfo CreateOption(PropertyInfo prop) => OptionInfo.FromMember(prop, GetAttributes(prop));

          #if NET_STANDARD_15
            IEnumerable<PropertyInfo> GetVerbOptions(MemberInfo source) =>
                (source as TypeInfo ?? ((PropertyInfo)source).PropertyType.GetTypeInfo()).GetProperties(BindingFlags.Public | BindingFlags.Instance);
          #else
            IEnumerable<PropertyInfo> GetVerbOptions(MemberInfo source) =>
                (source as Type ?? ((PropertyInfo)source).PropertyType).GetProperties(BindingFlags.Public | BindingFlags.Instance);
          #endif
        }

        public static ILookup<OptionInfo, ExampleInfo> GetExamplesFromOptionsRoots(List<Type> optionsRoots, IList<OptionInfo> options)
        {
            var lookup = new ExamplesLookup();
            var roots = GetTypeInfos(optionsRoots);
          #if CLP_19
            if (IsVersion19)
                return lookup;
          #endif
          #if CLP_22
            if (IsVersion22) {
                if (roots.Count > 1 || roots.SelectMany(GetAttributes).Any(a => a.GetType().FullName == VerbAttributeTypeName22)) {
                    foreach (var root in roots) {
                        lookup.Add(
                            options.FirstOrDefault(o => ReferenceEquals(o.SourceMember, root)),
                            GetExamplesFromOptions22(root.GetProperties(BindingFlags.Public | BindingFlags.Static)).ToList());
                    }
                }
                else if (roots.Count == 1)
                    lookup.Add(null, GetExamplesFromOptions22(roots.Single().GetProperties(BindingFlags.Public | BindingFlags.Static)).ToList());
                return lookup;
            }
          #endif
            throw UnsupportedVersion();
        }

        public static List<ErrorInfo> GetErrorsFromParserResultOrParserStateOrErrorList(object source)
        {
            List<ErrorInfo> errors = null;
            if (source is IEnumerable<ErrorInfo> errorList)
                errors = errorList.ToList();
            else if (source is ErrorInfo error)
                errors = new List<ErrorInfo>(1) { error };
          #if CLP_19
            else if (IsVersion19)
                errors = GetErrorsFromParserStateOrErrorList19(source).ToList();
          #endif
          #if CLP_22
            else if (IsVersion22)
                errors = GetErrorsFromParserResultOrErrorList22(source).ToList();
          #endif
            ErrorInfo.FormatSetErrors(errors);
            return errors ?? new List<ErrorInfo>();
        }

        public static Exception UnsupportedVersion() => new NotSupportedException("CommandLineParser version is not supported.");

        private static string Nullable(string value) => string.IsNullOrEmpty(value) ? null : value;
        private static IEnumerable<Attribute> GetAttributes(MemberInfo member) => member.GetCustomAttributes<Attribute>();
        private static IEnumerable<Attribute> GetAttributes(Type type) => type.GetCustomAttributes<Attribute>();
      #if NET_STANDARD_15
        internal static List<TypeInfo> GetTypeInfos(IEnumerable<Type> types) => types.Select(t => t.GetTypeInfo()).ToList();
        internal static TypeInfo GetTypeInfo(Type type) => type?.GetTypeInfo();
      #else
        internal static List<Type> GetTypeInfos(IEnumerable<Type> types) => types.ToList();
        internal static Type GetTypeInfo(Type type) => type;
      #endif

        // Closest we can get to ordered ReadOnlyDictionary<T> (which requires .NET 4.5+)
        private class ExamplesLookup : ILookup<OptionInfo, ExampleInfo>
        {
            private readonly List<ExamplesLookupItem> _items = new List<ExamplesLookupItem>();

            public int Count => _items.Count;
            IEnumerator<IGrouping<OptionInfo, ExampleInfo>> IEnumerable<IGrouping<OptionInfo, ExampleInfo>>.GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
            public bool Contains(OptionInfo key) => _items.Any(i => i.Option == key);
            public IEnumerable<ExampleInfo> this[OptionInfo key] => _items.FirstOrDefault(i => i.Option == key) ?? throw new KeyNotFoundException();
            public void Add(OptionInfo option, List<ExampleInfo> examples) => _items.Add(new ExamplesLookupItem { Option = option, Examples = examples });

            [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
            private class ExamplesLookupItem : IGrouping<OptionInfo, ExampleInfo>
            {
                public OptionInfo Option { get; set; }
                public List<ExampleInfo> Examples { get; set; }

                OptionInfo IGrouping<OptionInfo, ExampleInfo>.Key => Option;
                IEnumerator<ExampleInfo> IEnumerable<ExampleInfo>.GetEnumerator() => Examples.GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => Examples.GetEnumerator();
            }
        }
    }
}