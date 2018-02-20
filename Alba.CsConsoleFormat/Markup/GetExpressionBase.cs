using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        [CanBeNull]
        public object Source { get; set; }

        [CanBeNull]
        public string Path { get; set; }

        [CanBeNull]
        public CultureInfo Culture { get; set; }

        [CanBeNull]
        protected internal BindableObject TargetObject { get; set; }

        [CanBeNull]
        protected internal Type TargetType { get; set; }

        protected CultureInfo EffectiveCulture =>
            _effectiveCulture ?? (_effectiveCulture = Culture
             ?? (TargetObject as Element)?.EffectiveCulture
             ?? Thread.CurrentThread.CurrentCulture);

        [CanBeNull]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "'Value' is a conventional name for binding results.")]
        public object GetValue([CanBeNull] object targetObject = null)
        {
            object source = Source;
            if (source == null) {
                if (targetObject is BindableObject targetElement)
                    source = targetElement.DataContext;
                else if (targetObject != null)
                    source = targetObject;
                else if (TargetObject != null)
                    source = TargetObject.DataContext;
            }
            return GetValueFromSource(source);
        }

        [CanBeNull]
        protected abstract object GetValueFromSource([CanBeNull] object source);

        [CanBeNull]
        protected virtual object ConvertValue([CanBeNull] object value) => value;

        [CanBeNull]
        protected virtual object ConvertMethod([NotNull] MethodInfo method, [NotNull] object target) => throw new NotSupportedException();

        [CanBeNull]
        protected virtual object TryGetCachedMethod([NotNull] MethodInfo method) => null;

        private static object Get([NotNull] object value, [NotNull] string memberName)
        {
            if (!_getterFunctions.Value.TryGetValue(memberName, out GetterDelegate func))
                _getterFunctions.Value.Add(memberName, func = DynamicCaller.Get<object, object>(memberName));

            try {
                return func(value);
            }
            catch (RuntimeBinderException ex) {
                throw new InvalidOperationException($"Cannot resolve property or field '{memberName}'.", ex);
            }
        }

        [CanBeNull]
        internal object TraversePathToMethod([NotNull] object source)
        {
            string[] memberNames = Path?.Split('.') ?? new string[0];
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

        [CanBeNull]
        internal object TraversePathToProperty(object source)
        {
            object value = source;
            if (Path != null) {
                foreach (string memberName in Path.Split('.')) {
                    value = Get(value, memberName);
                    if (value == null)
                        return ConvertValue(null);
                }
            }
            return ConvertValue(value);
        }
    }
}