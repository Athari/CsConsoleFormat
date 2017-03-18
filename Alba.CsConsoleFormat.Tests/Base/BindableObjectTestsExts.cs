using System;
using System.Linq.Expressions;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;

namespace Alba.CsConsoleFormat.Tests
{
    internal static class BindableObjectTestsExts
    {
        public static T Bind<T, TTarget, TSource>(this T @this, Expression<Func<T, TTarget>> propertyExpr, Func<TSource, TTarget> getValue)
            where T : BindableObject
        {
            @this.Bind(
                (PropertyInfo)((MemberExpression)propertyExpr.Body).Member,
                new LambdaExpression<TSource, TTarget>(getValue));
            return @this;
        }

        private class LambdaExpression<TSource, TTarget> : GetExpressionBase
        {
            private readonly Func<TSource, TTarget> _getValue;
            public LambdaExpression(Func<TSource, TTarget> getValue) => _getValue = getValue;
            protected override object GetValueFromSource(object source) => _getValue(source.Should().BeAssignableTo<TSource>().Subject);
        }
    }
}