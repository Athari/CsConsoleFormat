using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Framework.Reflection;
using JetBrains.Annotations;
using Microsoft.CSharp.RuntimeBinder;

namespace Alba.CsConsoleFormat.Markup
{
    using GetterDelegate = Func<Object, Object>;
    using GetterDelegatesCache = Dictionary<String, Func<Object, Object>>;

    public abstract class GetExpressionBase
    {
        private static readonly ThreadLocal<GetterDelegatesCache> _getterFunctions =
            new ThreadLocal<GetterDelegatesCache>(() => new GetterDelegatesCache());

        private CultureInfo _effectiveCulture;

        public object Source { get; set; }
        public string Path { get; set; }
        public CultureInfo Culture { get; set; }
        public BindableObject TargetObject { get; set; }
        public Type TargetType { get; set; }

        protected CultureInfo EffectiveCulture =>
            _effectiveCulture ?? (_effectiveCulture = Culture
                ?? (TargetObject as Element)?.EffectiveCulture
                    ?? Thread.CurrentThread.CurrentCulture);

        public object GetValue (object targetObject = null)
        {
            object source = Source;
            if (source == null) {
                var targetElement = targetObject as BindableObject;
                if (targetElement != null)
                    source = targetElement.DataContext;
                else if (targetObject != null)
                    source = targetObject;
                else if (TargetObject != null)
                    source = TargetObject.DataContext;
            }
            return GetValueFromSource(source);
        }

        protected abstract object GetValueFromSource (object source);

        protected virtual object ConvertValue (object value)
        {
            return value;
        }

        protected virtual object ConvertMethod ([NotNull] MethodInfo method, [NotNull] object target)
        {
            throw new NotSupportedException();
        }

        protected virtual object TryGetCachedMethod (MethodInfo method)
        {
            return null;
        }

        internal static object Get (object value, string memberName)
        {
            GetterDelegate func;
            if (!_getterFunctions.Value.TryGetValue(memberName, out func)) {
                func = DynamicCaller.Get<object, object>(memberName);
                _getterFunctions.Value.Add(memberName, func);
            }
            try {
                return func(value);
            }
            catch (RuntimeBinderException ex) {
                throw new InvalidOperationException($"Cannot resolve property or field '{memberName}'.", ex);
            }
        }

        internal object TraversePathToMethod (object source)
        {
            string[] memberNames = Path.Split('.');
            string memberName;
            object value = source;
            for (int i = 0; i < memberNames.Length - 1; i++) {
                memberName = memberNames[i];
                value = Get(value, memberName);
                if (value == null)
                    throw new InvalidOperationException($"Property or field '{memberName}' cannot be null.");
            }

            memberName = memberNames[memberNames.Length - 1];
            Type valueType = value.GetType();
            MethodInfo method;
            try {
                method = valueType.GetMethod(memberName);
            }
            catch (AmbiguousMatchException) {
                MethodInfo[] allMethods = valueType.GetMethods();
                method = allMethods.First(m => m.Name == memberName);

                object func = TryGetCachedMethod(method);
                if (func != null)
                    return func;

                List<MethodInfo> candidateMethods = allMethods.Where(m => m.Name == memberName).ToList();
                if (!candidateMethods.Select(m => m.IsStatic).AllEqual() || !candidateMethods.Select(m => m.GetParameters().Length).AllEqual())
                    throw new InvalidOperationException($"All overloads of '{memberName}' must have compatible signatures.");
            }
            if (method != null)
                return ConvertMethod(method, value);
            try {
                return ConvertValue(Get(value, memberName));
            }
            catch (InvalidOperationException ex) {
                throw new InvalidOperationException($"Cannot resolve method, property or field '{memberName}'.", ex);
            }
        }

        internal object TraversePathToProperty (object source)
        {
            object value = source;
            foreach (string memberName in Path.Split('.')) {
                value = Get(value, memberName);
                if (value == null)
                    return ConvertValue(null);
            }
            return ConvertValue(value);
        }
    }
}