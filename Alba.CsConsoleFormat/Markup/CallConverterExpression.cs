using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Alba.CsConsoleFormat.Framework.Reflection;
using Alba.CsConsoleFormat.Framework.Text;

// ReSharper disable BuiltInTypeReferenceStyle
namespace Alba.CsConsoleFormat.Markup
{
    using ConverterDelegatesCache = Dictionary<MethodInfo, ConverterFunc>;

    public delegate object ConverterFunc(object value, object parameter, CultureInfo culture);

    public class CallConverterExpression : GetExpressionBase
    {
        private static readonly ThreadLocal<ConverterDelegatesCache> _converterFunctions =
            new ThreadLocal<ConverterDelegatesCache>(() => new ConverterDelegatesCache());

        protected override object GetValueFromSource(object source)
        {
            if (source == null)
                throw new InvalidOperationException("Source cannot be null.");
            if (Path.IsNullOrEmpty())
                return ConvertValue(source);
            return TraversePathToMethod(source);
        }

        protected override object TryGetCachedMethod(MethodInfo method)
        {
            return _converterFunctions.Value.TryGetValue(method, out ConverterFunc func) ? func : null;
        }

        protected override object ConvertValue(object value)
        {
            switch (value) {
                case Func<object, object, object, CultureInfo> func3:
                    return (ConverterFunc)((v, p, c) => func3(v, p, c));
                case Func<object, object, object> func2:
                    return (ConverterFunc)((v, p, c) => func2(v, p));
                case Func<object, object> func1:
                    return (ConverterFunc)((v, p, c) => func1(v));
                default:
                    throw new InvalidOperationException("Cannot cast value to converter delegate.");
            }
        }

        protected override object ConvertMethod(MethodInfo method, object target)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!_converterFunctions.Value.TryGetValue(method, out ConverterFunc func)) {
                func = GetConverterFunc();
                _converterFunctions.Value.Add(method, func);
            }
            return func;

            ConverterFunc GetConverterFunc()
            {
                ParameterInfo[] parameters = method.GetParameters();
                Type targetType = target.GetType();
                CultureInfo culture = EffectiveCulture;

                if (method.IsStatic) {
                    switch (parameters.Length) {
                        case 3:
                            return (v, p, c) => DynamicCaller.CallStatic<Func<Type, object, object, CultureInfo, object>>(method.Name)(targetType, v, p, c ?? culture);
                        case 2:
                            return (v, p, c) => DynamicCaller.CallStatic<Func<Type, object, object, object>>(method.Name)(targetType, v, p);
                        case 1:
                            return (v, p, c) => DynamicCaller.CallStatic<Func<Type, object, object>>(method.Name)(targetType, v);
                    }
                }
                else {
                    switch (parameters.Length) {
                        case 3:
                            return (v, p, c) => DynamicCaller.Call<Func<Object, object, object, CultureInfo, object>>(method.Name)(target, v, p, c ?? culture);
                        case 2:
                            return (v, p, c) => DynamicCaller.Call<Func<Object, object, object, object>>(method.Name)(target, v, p);
                        case 1:
                            return (v, p, c) => DynamicCaller.Call<Func<Object, object, object>>(method.Name)(target, v);
                    }
                }
                return null;
            }
        }
    }
}