using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
#if SYSTEM_XAML
using System.Xaml;
#else
using Portable.Xaml;
#endif

// ReSharper disable UseObjectOrCollectionInitializer
namespace Alba.CsConsoleFormat.Tests
{
    public sealed class BindableObjectTests
    {
        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("ReSharper", "LocalNameCapturedOnly")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public void NullArguments()
        {
            var obj = new MyBindableObject();
            var property = obj.GetType().GetProperty(nameof(MyBindableObject.MyStringValue));

          #if XAML
            var getter = new GetExpression();

            new Action(() => obj.Bind(null, getter)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.Bind(property, null)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(getter));
          #endif
            new Action(() => _ = obj.HasValue<int>(null)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => _ = obj.GetValue<int>(null)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.ResetValue<int>(null)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.SetValue(null, 1)).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj[null] = 1).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => _ = obj[null].As<int>()).Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
        }

        #if XAML

        [Fact]
        public void EmptyDataContext()
        {
            var obj = new MyBindableObject();

            new Action(() => obj.DataContext = null).Should().NotThrow();
        }

        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Reflection is guaranteed to succeed due to nameof.")]
        public void Bind()
        {
            var obj = new MyBindableObject();
            var data = new Data { StringValue = "str", Int32Value = 42 };

            obj.Bind(
                obj.GetType().GetProperty(nameof(MyBindableObject.MyStringValue)),
                new GetExpression { Path = nameof(Data.StringValue) });
            obj.Bind(
                obj.GetType().GetProperty(nameof(MyBindableObject.MyInt32Value)),
                new GetExpression { Path = nameof(Data.Int32Value) });
            obj.DataContext = data;

            obj.MyStringValue.Should().Be(data.StringValue);
            obj.MyInt32Value.Should().Be(data.Int32Value);
        }

        [Fact]
        public void DataContextWithotBind()
        {
            var obj = new MyBindableObject();
            var data = new Data { StringValue = "str", Int32Value = 42 };

            obj.DataContext = data;

            obj.MyStringValue.Should().Be(default);
            obj.MyInt32Value.Should().Be(default);
        }

        #endif

        [Fact]
        public void Clone()
        {
            var obj = new MyBindableObject { MyStringValue = "str", MyInt32Value = 42 };

            var clone = (MyBindableObject)obj.Clone();

            clone.Should().NotBeSameAs(obj);
            clone.MyStringValue.Should().Be(obj.MyStringValue);
            clone.MyInt32Value.Should().Be(obj.MyInt32Value);
        }

        [Fact]
        public void CloneAttachedProperties()
        {
            var obj = new MyBindableObject { [MyBindableObject.AttachedDecimalProperty] = 10m };

            var clone = (MyBindableObject)obj.Clone();
            obj[MyBindableObject.AttachedDecimalProperty] = 20m;

            clone[MyBindableObject.AttachedDecimalProperty].Should().Be(10m);
        }

        [Fact]
        public void CloneWithCreateInstance()
        {
            var obj = new MyCloneableObject("MyValue");

            var clone = (MyCloneableObject)obj.Clone();

            obj.IsCloned.Should().BeFalse();
            clone.Value.Should().Be("MyValueCloned");
            clone.IsCloned.Should().BeTrue();
        }

        [Fact]
        public void UnsetValue()
        {
            var obj = new MyBindableObject();

            AssertDefaultValue(obj, 1m);
        }

        [Fact]
        public void SetValue()
        {
            var obj = new MyBindableObject();

            obj.SetValue(MyBindableObject.AttachedDecimalProperty, 2m);

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void SetValueWithIndexer()
        {
            var obj = new MyBindableObject();

            obj[MyBindableObject.AttachedDecimalProperty] = 2m;

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void ResetValue()
        {
            var obj = new MyBindableObject();

            obj.SetValue(MyBindableObject.AttachedDecimalProperty, 2m);
            obj.ResetValue(MyBindableObject.AttachedDecimalProperty);

            AssertDefaultValue(obj, 1m);
        }

        #if XAML
        [Fact]
        public void SetValueWithSetProperty()
        {
            var obj = new MyBindableObject();

            obj.As<IAttachedPropertyStore>().SetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, 2m);

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void ResetValueWithRemoveProperty()
        {
            var obj = new MyBindableObject();

            obj.As<IAttachedPropertyStore>().SetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, 2m);
            obj.As<IAttachedPropertyStore>().RemoveProperty(MyBindableObject.AttachedDecimalProperty.Identifier);

            AssertDefaultValue(obj, 1m);
        }

        #endif

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void AssertDefaultValue([NotNull] MyBindableObject obj, decimal expectedValue)
        {
            obj.HasValue(MyBindableObject.AttachedDecimalProperty).Should().BeFalse();
            obj.GetValue(MyBindableObject.AttachedDecimalProperty).Should().Be(expectedValue);
            obj[MyBindableObject.AttachedDecimalProperty].Should().Be(expectedValue);

          #if XAML
            obj.As<IAttachedPropertyStore>().PropertyCount.Should().Be(0);
            obj.As<IAttachedPropertyStore>().TryGetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, out object value);
            value.Should().Be(expectedValue);
          #endif
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void AssertCustomValue([NotNull] MyBindableObject obj, decimal expectedValue)
        {
            obj.HasValue(MyBindableObject.AttachedDecimalProperty).Should().BeTrue();
            obj.GetValue(MyBindableObject.AttachedDecimalProperty).Should().Be(expectedValue);
            obj[MyBindableObject.AttachedDecimalProperty].Should().Be(expectedValue);

          #if XAML
            var properties = new KeyValuePair<AttachableMemberIdentifier, object>[1];

            obj.As<IAttachedPropertyStore>().PropertyCount.Should().Be(1);
            obj.As<IAttachedPropertyStore>().TryGetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, out object value);
            value.Should().Be(expectedValue);
            obj.As<IAttachedPropertyStore>().CopyPropertiesTo(properties, 0);
            properties[0].Should().Be(new KeyValuePair<AttachableMemberIdentifier, object>(MyBindableObject.AttachedDecimalProperty.Identifier, expectedValue));
          #endif
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        private sealed class MyBindableObject : BindableObject
        {
            public static readonly AttachedProperty<decimal> AttachedDecimalProperty =
                AttachedProperty.Register<MyBindableObject, decimal>(nameof(AttachedDecimalProperty), 1m);

            public string MyStringValue { get; set; }
            public int MyInt32Value { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        private sealed class MyCloneableObject : BindableObject
        {
            public string Value { get; set; }
            public bool IsCloned { get; }

            private MyCloneableObject(string value, bool isCloned)
            {
                Value = value;
                IsCloned = isCloned;
            }

            public MyCloneableObject(string value) : this(value, false)
            { }

            protected override BindableObject CreateInstance() => new MyCloneableObject(Value + "Cloned", true);
        }

        [UsedImplicitly]
        public sealed class Data
        {
            public string StringValue { get; set; }
            public int Int32Value { get; set; }
        }
    }
}