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
    using ConverterDelegatesCache = Dictionary<MethodInfo, ConverterDelegate>;

    public delegate object ConverterDelegate(object value, object parameter, CultureInfo culture);

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
            return _converterFunctions.Value.TryGetValue(method, out ConverterDelegate func) ? func : null;
        }

        protected override object ConvertValue(object value)
        {
            switch (value) {
                case Func<object, object, object, CultureInfo> func3:
                    return (ConverterDelegate)((v, p, c) => func3(v, p, c));
                case Func<object, object, object> func2:
                    return (ConverterDelegate)((v, p, c) => func2(v, p));
                case Func<object, object> func1:
                    return (ConverterDelegate)((v, p, c) => func1(v));
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

            if (_converterFunctions.Value.TryGetValue(method, out ConverterDelegate func))
                return func;

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
            return func;
        }
    }
}