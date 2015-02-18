using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Framework.Reflection;
using Alba.CsConsoleFormat.Framework.Text;

namespace Alba.CsConsoleFormat.Markup
{
    using ConverterDelegate = Func<Object, Object, CultureInfo, Object>;
    using ConverterDelegatesCache = Dictionary<MethodInfo, Func<Object, Object, CultureInfo, Object>>;

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
                    MethodInfo method;
                    try {
                        method = valueType.GetMethod(memberName);
                    }
                    catch (AmbiguousMatchException) {
                        MethodInfo[] allMethods = valueType.GetMethods();
                        method = allMethods.First(m => m.Name == memberName);

                        ConverterDelegate func;
                        if (_converterFunctions.Value.TryGetValue(method, out func))
                            return func;

                        List<MethodInfo> candidateMethods = allMethods.Where(m => m.Name == memberName).ToList();
                        if (!candidateMethods.Select(m => m.IsStatic).AllEqual() || !candidateMethods.Select(m => m.GetParameters().Length).AllEqual())
                            throw new InvalidOperationException("All overloads of '{0}' must have the same signature.".FmtInv(memberName));
                    }
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
            var func3 = value as Func<object, object, CultureInfo, object>;
            if (func3 != null)
                return func3;
            var func2 = value as Func<object, object, object>;
            if (func2 != null)
                return (v, p, c) => func2(v, p);
            var func1 = value as Func<object, object>;
            if (func1 != null)
                return (v, p, c) => func1(v);
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
                    if (parameters.Length == 3) {
                        var call = DynamicCaller.CallStatic<Func<Type, object, object, CultureInfo, object>>(method.Name);
                        func = (v, p, c) => call(targetType, v, p, c ?? culture);
                    }
                    else if (parameters.Length == 2) {
                        var call = DynamicCaller.CallStatic<Func<Type, object, object, object>>(method.Name);
                        func = (v, p, c) => call(targetType, v, p);
                    }
                    else if (parameters.Length == 1) {
                        var call = DynamicCaller.CallStatic<Func<Type, object, object>>(method.Name);
                        func = (v, p, c) => call(targetType, v);
                    }
                }
                else {
                    CultureInfo culture = EffectiveCulture;
                    if (parameters.Length == 3) {
                        var call = DynamicCaller.Call<Func<Object, object, object, CultureInfo, object>>(method.Name);
                        func = (v, p, c) => call(target, v, p, c ?? culture);
                    }
                    else if (parameters.Length == 2) {
                        var call = DynamicCaller.Call<Func<Object, object, object, object>>(method.Name);
                        func = (v, p, c) => call(target, v, p);
                    }
                    else if (parameters.Length == 1) {
                        var call = DynamicCaller.Call<Func<Object, object, object>>(method.Name);
                        func = (v, p, c) => call(target, v);
                    }
                }
                _converterFunctions.Value.Add(method, func);
            }
            return func;
        }
    }
}