using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Alba.CsConsoleFormat.Framework.Reflection;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat.Markup
{
    using ConverterDelegate = Func<Object, CultureInfo, Object>;
    using ConverterDelegatesCache = Dictionary<MethodInfo, Func<Object, CultureInfo, Object>>;

    public class CallExpression : GetExpressionBase
    {
        private static readonly ThreadLocal<ConverterDelegatesCache> _converterFunctions =
            new ThreadLocal<ConverterDelegatesCache>(() => new ConverterDelegatesCache());

        protected override object GetValueFromSource (object source)
        {
            if (source == null)
                throw new InvalidOperationException("Source cannot be null.");

            if (Path.IsNullOrEmpty())
                return ConvertValueToConverter(source);

            string[] memberNames = Path.Split('.');
            object value = source;
            for (int i = 0; i < memberNames.Length; i++) {
                string memberName = memberNames[i];
                Type valueType = value.GetType();
                if (i < memberNames.Length - 1) {
                    value = Get(value, memberName);
                    if (value == null)
                        throw new InvalidOperationException("Property or field '{0}' cannot be null.".FmtInv(memberName));
                }
                else {
                    MethodInfo method = valueType.GetMethod(memberName);
                    if (method != null)
                        return ConvertMethodToConverter(method, value);
                    try {
                        return ConvertValueToConverter(Get(value, memberName));
                    }
                    catch (InvalidOperationException ex) {
                        throw new InvalidOperationException("Cannot resolve method, property or field '{0}'.".FmtInv(memberName), ex);
                    }
                }
            }
            throw new InvalidOperationException();
        }

        private ConverterDelegate ConvertValueToConverter (object value)
        {
            var func1 = value as Func<object, CultureInfo, object>;
            if (func1 != null)
                return func1;
            var func2 = value as Func<object, object>;
            if (func2 != null)
                return (v, c) => func2(v);
            throw new InvalidOperationException("Cannot cast value to converter delegate.");
        }

        private ConverterDelegate ConvertMethodToConverter (MethodInfo method, object target)
        {
            ConverterDelegate func;
            if (!_converterFunctions.Value.TryGetValue(method, out func)) {
                ParameterInfo[] parameters = method.GetParameters();
                Type targetType = target.GetType();
                if (method.IsStatic) {
                    CultureInfo culture = EffectiveCulture;
                    if (parameters.Length == 2) {
                        var call = DynamicCaller.CallStatic<Func<Type, object, CultureInfo, object>>(method.Name);
                        func = (v, c) => call(targetType, v, c ?? culture);
                    }
                    else if (parameters.Length == 1) {
                        var call = DynamicCaller.CallStatic<Func<Type, object, object>>(method.Name);
                        func = (v, c) => call(targetType, v);
                    }
                }
                else {
                    CultureInfo culture = EffectiveCulture;
                    if (parameters.Length == 2) {
                        var call = DynamicCaller.Call<Func<Object, object, CultureInfo, object>>(method.Name);
                        func = (v, c) => call(target, v, c ?? culture);
                    }
                    else if (parameters.Length == 1) {
                        var call = DynamicCaller.Call<Func<Object, object, object>>(method.Name);
                        func = (v, c) => call(target, v);
                    }
                }
                _converterFunctions.Value.Add(method, func);
            }
            return func;
        }
    }
}