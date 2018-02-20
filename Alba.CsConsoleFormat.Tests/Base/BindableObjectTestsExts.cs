using System;
using System.Linq.Expressions;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Tests
{
    internal static class BindableObjectTestsExts
    {
        public static T Bind<T, TTarget, TSource>([NotNull] this T @this, [NotNull] Expression<Func<T, TTarget>> propertyExpr, [NotNull] Func<TSource, TTarget> getValue)
            where T : BindableObject
        {
            @this.Bind(
                (PropertyInfo)((MemberExpression)propertyExpr.Body).Member,
                new LambdaExpression<TSource, TTarget>(getValue));
            return @this;
        }

        private sealed class LambdaExpression<TSource, TTarget> : GetExpressionBase
        {
            private readonly Func<TSource, TTarget> _getValue;
            public LambdaExpression(Func<TSource, TTarget> getValue) => _getValue = getValue;
            protected override object GetValueFromSource(object source) => _getValue(source.Should().BeAssignableTo<TSource>().Subject);
        }
    }
}