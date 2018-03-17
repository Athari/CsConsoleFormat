using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if NET_40 || NET_STANDARD_15
using Alba.CsConsoleFormat.CommandLineParser.Framework;
#endif

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public sealed class OptionListInfo
    {
        private readonly List<OptionInfo> _options = new List<OptionInfo>();

        public IList<OptionInfo> Options => _options;

        internal OptionListInfo()
        { }

        public static OptionListInfo From(params Type[] roots)
        {
            var info = new OptionListInfo();
            info._options.AddRange(GetOptionsFromRoots(roots)
                .OrderBy(o => o.IsPositional ? o.Index : int.MaxValue)
                .ThenBy(o => o.Name));
            return info;
        }

        private static IEnumerable<OptionInfo> GetOptionsFromRoots(Type[] roots)
        {
            // 2.2 - Verbs
            if (roots.Length > 1 || roots.SelectMany(GetAttributes).Any(a => a.GetType().FullName == "CommandLine.VerbAttribute"))
                return roots.Select(CreateOptionVerb22);
            if (roots.Length == 1) {
                var props = roots.Single().GetProperties();
                // 1.9 - Verbs
              #if CLP_19
                if (props.SelectMany(GetAttributes).Any(a => a.GetType().FullName == "CommandLine.VerbOptionAttribute"))
                    return props.Select(CreateOptionVerb19);
              #endif
                // 2.2 - Simple
                if (props.SelectMany(GetAttributes).Any(a => a.GetType().GetBaseType()?.FullName == "CommandLine.BaseAttribute"))
                    return props.Select(CreateOption22);
              #if CLP_19
                else
                    return props.Select(CreateOption19);
              #endif
            }
            throw new NotSupportedException("CommandLine version or configuration not supported.");

            OptionInfo CreateOptionVerb22(Type root) => new OptionInfo22(GetAttributes(root), root.Name, isVerb: true)
                .WithSubOptions(root.GetProperties().Select(CreateOption22));

            OptionInfo CreateOption22(PropertyInfo prop) => new OptionInfo22(GetAttributes(prop), prop.Name);

          #if CLP_19
            OptionInfo CreateOptionVerb19(PropertyInfo prop) => new OptionInfo19(GetAttributes(prop), prop.Name, isVerb: true)
                .WithSubOptions(prop.PropertyType.GetProperties().Select(CreateOption19));

            OptionInfo CreateOption19(PropertyInfo prop) => new OptionInfo19(GetAttributes(prop), prop.Name);
          #endif
        }

        private static IEnumerable<Attribute> GetAttributes(MemberInfo member) => member.GetCustomAttributes<Attribute>();
        private static IEnumerable<Attribute> GetAttributes(Type type) => type.GetCustomAttributes<Attribute>();
    }
}