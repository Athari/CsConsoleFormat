using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Alba.CsConsoleFormat.Framework.Reflection
{
    internal static class DynamicCaller
    {
        public static TDelegate Call<TDelegate> (string memberName, object context = null, Type[] genericArgs = null)
        {
            MethodInfo method = GetDelegateMethod<TDelegate>();
            ParameterInfo[] methodParams = method.GetParameters();
            CallSite callSite = CallSite.Create(
                CreateDelegateType(method, methodParams),
                Binder.InvokeMember(
                    method.IsVoid() ? CSharpBinderFlags.ResultDiscarded : CSharpBinderFlags.None,
                    memberName, genericArgs ?? Type.EmptyTypes, Context(context), Args(methodParams.Length, false)));
            return CreateDelegateInvokeCallSite<TDelegate>(method, methodParams, callSite);
        }

        public static TDelegate CallStatic<TDelegate> (string memberName, object context = null, Type[] genericArgs = null)
        {
            MethodInfo method = GetDelegateMethod<TDelegate>();
            ParameterInfo[] methodParams = method.GetParameters();
            CallSite callSite = CallSite.Create(
                CreateDelegateType(method, methodParams),
                Binder.InvokeMember(
                    method.IsVoid() ? CSharpBinderFlags.ResultDiscarded : CSharpBinderFlags.None,
                    memberName, genericArgs ?? Type.EmptyTypes, Context(context), Args(methodParams.Length, true)));
            return CreateDelegateInvokeCallSite<TDelegate>(method, methodParams, callSite);
        }

        public static Func<TObject, TProperty> Get<TObject, TProperty> (string memberName, object context = null)
        {
            var callSite = CallSite<Func<CallSite, TObject, object>>.Create(
                Binder.GetMember(CSharpBinderFlags.None, memberName, Context(context), new[] { Arg }));
            return obj => (TProperty)callSite.Target(callSite, obj);
        }

        public static Action<TObject, TProperty> Set<TObject, TProperty> (string memberName, object context = null)
        {
            var callSite = CallSite<Action<CallSite, TObject, TProperty>>.Create(
                Binder.SetMember(CSharpBinderFlags.ResultDiscarded, memberName, Context(context), new[] { Arg, Arg }));
            return (obj, value) => callSite.Target(callSite, obj, value);
        }

        private static Type CreateDelegateType (MethodInfo method, ParameterInfo[] methodParams)
        {
            var delegateParams = new Type[methodParams.Length + 2];
            delegateParams[0] = typeof(CallSite);
            for (int i = 0; i < methodParams.Length; i++)
                delegateParams[i + 1] = methodParams[i].ParameterType;
            delegateParams[delegateParams.Length - 1] = method.IsVoid() ? typeof(void) : typeof(object);
            return Expression.GetDelegateType(delegateParams);
        }

        private static TDelegate CreateDelegateInvokeCallSite<TDelegate> (MethodInfo method, ParameterInfo[] methodParams, CallSite callSite)
        {
            List<ParameterExpression> exprParams = methodParams.Select(p => Expression.Parameter(p.ParameterType)).ToList();
            Expression exprInvokeCallSiteTarget = Expression.Invoke(
                Expression.Field(Expression.Constant(callSite), "Target"),
                Enumerable.Repeat((Expression)Expression.Constant(callSite), 1).Concat(exprParams)
                );
            Expression<TDelegate> call = Expression.Lambda<TDelegate>(
                method.IsVoid() || method.ReturnType == typeof(object)
                    ? exprInvokeCallSiteTarget
                    : Expression.Convert(exprInvokeCallSiteTarget, method.ReturnType),
                exprParams);
            return call.Compile();
        }

        private static IEnumerable<CSharpArgumentInfo> Args (int nArgs, bool isStaticContext)
        {
            if (isStaticContext)
                yield return ArgCompileTimeStatic;
            for (int i = 0; i < nArgs; i++)
                yield return Arg;
        }

        private static CSharpArgumentInfo ArgCompileTimeStatic
        {
            get { return CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null); }
        }

        private static CSharpArgumentInfo Arg
        {
            get { return CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null); }
        }

        private static Type Context (object context)
        {
            return context is Type ? (Type)context : context != null ? context.GetType() : typeof(object);
        }

        private static MethodInfo GetDelegateMethod<TDelegate> ()
        {
            return typeof(TDelegate).GetMethod("Invoke");
        }
    }
}