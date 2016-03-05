using System;
using Alba.CsConsoleFormat.Framework.Reflection;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests.Framework.Reflection
{
    public class DynamicCallerTests
    {
        [Fact]
        public void CallInstanceMethodSimple ()
        {
            DynamicCaller.Call<Func<Foo, object>>(nameof(Foo.InstanceMethod0)).Invoke(new Foo()).Should().Be(1);
            DynamicCaller.Call<Func<Foo, int>>(nameof(Foo.InstanceMethod0)).Invoke(new Foo()).Should().Be(1);
        }

        [Fact]
        public void CallInstanceMethodWithArguments ()
        {
            DynamicCaller.Call<Func<Foo, object, object, object>>(nameof(Foo.InstanceMethod1)).Invoke(new Foo(), 1, 2).Should().Be(3);
            DynamicCaller.Call<Func<Foo, object, object, int>>(nameof(Foo.InstanceMethod1)).Invoke(new Foo(), 1, 2).Should().Be(3);
            DynamicCaller.Call<Func<Foo, int, int, int>>(nameof(Foo.InstanceMethod1)).Invoke(new Foo(), 1, 2).Should().Be(3);
        }

        [Fact]
        public void CallInstanceMethodWithOverloads ()
        {
            const int DoubleResult = 4, IntResult = 3;
            DynamicCaller.Call<Func<Foo, object, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, object, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, object, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5).Should().Be(IntResult);
            DynamicCaller.Call<Func<Foo, object, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, object, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, object, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5).Should().Be(IntResult);
            DynamicCaller.Call<Func<Foo, double, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, double, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, double, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, double, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, double, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5.0).Should().Be(DoubleResult);
            DynamicCaller.Call<Func<Foo, int, object>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5).Should().Be(IntResult);
            DynamicCaller.Call<Func<Foo, int, int>>(nameof(Foo.InstanceMethod2)).Invoke(new Foo(), 5).Should().Be(IntResult);
        }

        [Fact]
        public void CallStaticMethodSimple ()
        {
            DynamicCaller.CallStatic<Func<Type, object>>(nameof(Foo.StaticMethod0)).Invoke(typeof(Foo)).Should().Be(1);
            DynamicCaller.CallStatic<Func<Type, int>>(nameof(Foo.StaticMethod0)).Invoke(typeof(Foo)).Should().Be(1);
        }

        [Fact]
        public void CallStaticMethodWithArguments ()
        {
            DynamicCaller.CallStatic<Func<Type, object, object, object>>(nameof(Foo.StaticMethod1)).Invoke(typeof(Foo), 1, 2).Should().Be(3);
            DynamicCaller.CallStatic<Func<Type, object, object, int>>(nameof(Foo.StaticMethod1)).Invoke(typeof(Foo), 1, 2).Should().Be(3);
            DynamicCaller.CallStatic<Func<Type, int, int, int>>(nameof(Foo.StaticMethod1)).Invoke(typeof(Foo), 1, 2).Should().Be(3);
        }

        [Fact]
        public void CallStaticMethodWithOverloads ()
        {
            const int DoubleResult = 4, IntResult = 3;
            DynamicCaller.CallStatic<Func<Type, object, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, object, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, object, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5).Should().Be(IntResult);
            DynamicCaller.CallStatic<Func<Type, object, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, object, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, object, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5).Should().Be(IntResult);
            DynamicCaller.CallStatic<Func<Type, double, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, double, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, double, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, double, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0f).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, double, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5.0).Should().Be(DoubleResult);
            DynamicCaller.CallStatic<Func<Type, int, object>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5).Should().Be(IntResult);
            DynamicCaller.CallStatic<Func<Type, int, int>>(nameof(Foo.StaticMethod2)).Invoke(typeof(Foo), 5).Should().Be(IntResult);
        }

        [Fact]
        public void CallInstancePropertyGetter ()
        {
            var foo = new Foo { Property = 1 };
            DynamicCaller.Get<object, object>(nameof(Foo.Property)).Invoke(foo).Should().Be(1);
            DynamicCaller.Get<object, int>(nameof(Foo.Property)).Invoke(foo).Should().Be(1);
            DynamicCaller.Get<Foo, object>(nameof(Foo.Property)).Invoke(foo).Should().Be(1);
            DynamicCaller.Get<Foo, int>(nameof(Foo.Property)).Invoke(foo).Should().Be(1);
        }

        [Fact]
        public void CallInstancePropertySetter ()
        {
            var foo = new Foo { Property = 1 };
            DynamicCaller.Set<object, object>(nameof(Foo.Property)).Invoke(foo, 2);
            foo.Property.Should().Be(2);
            DynamicCaller.Set<object, int>(nameof(Foo.Property)).Invoke(foo, 3);
            foo.Property.Should().Be(3);
            DynamicCaller.Set<Foo, object>(nameof(Foo.Property)).Invoke(foo, 4);
            foo.Property.Should().Be(4);
            DynamicCaller.Set<Foo, int>(nameof(Foo.Property)).Invoke(foo, 5);
            foo.Property.Should().Be(5);
        }

        public class Foo
        {
            public int Property { get; set; }

            public int InstanceMethod0 () => 1;
            public int InstanceMethod1 (int a, int b) => a + b;
            public int InstanceMethod2 (int a) => 3;
            public int InstanceMethod2 (double a) => 4;

            public static int StaticMethod0 () => 1;
            public static int StaticMethod1 (int a, int b) => a + b;
            public static int StaticMethod2 (int a) => 3;
            public static int StaticMethod2 (double a) => 4;
        }
    }
}