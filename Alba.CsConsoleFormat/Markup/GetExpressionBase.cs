using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Alba.CsConsoleFormat.Framework.Reflection;
using Alba.CsConsoleFormat.Framework.Text;
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
        public Element TargetObject { get; set; }
        public Type TargetType { get; set; }

        protected CultureInfo EffectiveCulture
        {
            get
            {
                return _effectiveCulture ?? (_effectiveCulture = Culture
                    ?? (TargetObject != null ? TargetObject.EffectiveCulture : null)
                        ?? Thread.CurrentThread.CurrentCulture);
            }
        }

        public object GetValue (object targetObject = null)
        {
            object source = Source;
            if (source == null) {
                var targetElement = targetObject as Element;
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

        internal object Get (object value, string memberName)
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
                throw new InvalidOperationException("Cannot resolve property or field '{0}'.".FmtInv(memberName), ex);
            }
        }
    }
}