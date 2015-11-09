using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xaml;
using Alba.CsConsoleFormat.Markup;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

// ReSharper disable UseObjectOrCollectionInitializer
namespace Alba.CsConsoleFormat.Tests
{
    public class BindableObjectTests
    {
        [Fact]
        public void EmptyDataContext ()
        {
            var obj = new MyBindableObject();

            new Action(() => obj.DataContext = null).ShouldNotThrow();
        }

        [Fact]
        [SuppressMessage ("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage ("ReSharper", "UnusedVariable")]
        public void NullArguments ()
        {
            var obj = new MyBindableObject();
            var getter = new GetExpression();
            var prop = obj.GetType().GetProperty(nameof(MyBindableObject.MyStringValue));
            var property = MyBindableObject.AttachedDecimalProperty;

            new Action(() => obj.Bind(null, getter)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(prop));
            new Action(() => obj.Bind(prop, null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(getter));
            new Action(() => obj.HasValue<int>(null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.GetValue<int>(null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.ResetValue<int>(null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj.SetValue(null, 1)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => obj[null] = 1).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
            new Action(() => { var a = obj[null]; }).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(property));
        }

        [Fact]
        public void Bind ()
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
        public void DataContextWithotBind ()
        {
            var obj = new MyBindableObject();
            var data = new Data { StringValue = "str", Int32Value = 42 };

            obj.DataContext = data;

            obj.MyStringValue.Should().Be(default(string));
            obj.MyInt32Value.Should().Be(default(int));
        }

        [Fact]
        public void Clone ()
        {
            var obj = new MyBindableObject { MyStringValue = "str", MyInt32Value = 42 };

            var clone = (MyBindableObject)obj.Clone();

            clone.MyStringValue.Should().Be(obj.MyStringValue);
            clone.MyInt32Value.Should().Be(obj.MyInt32Value);
        }

        [Fact]
        public void UnsetValue ()
        {
            var obj = new MyBindableObject();

            AssertDefaultValue(obj, 1m);
        }

        [Fact]
        public void SetValue ()
        {
            var obj = new MyBindableObject();

            obj.SetValue(MyBindableObject.AttachedDecimalProperty, 2m);

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void SetValueWithIndexer ()
        {
            var obj = new MyBindableObject();

            obj[MyBindableObject.AttachedDecimalProperty] = 2m;

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void SetValueWithSetProperty ()
        {
            var obj = new MyBindableObject();

            obj.As<IAttachedPropertyStore>().SetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, 2m);

            AssertCustomValue(obj, 2m);
        }

        [Fact]
        public void ResetValue ()
        {
            var obj = new MyBindableObject();

            obj.SetValue(MyBindableObject.AttachedDecimalProperty, 2m);
            obj.ResetValue(MyBindableObject.AttachedDecimalProperty);

            AssertDefaultValue(obj, 1m);
        }

        [Fact]
        public void ResetValueWithRemoveProperty ()
        {
            var obj = new MyBindableObject();

            obj.As<IAttachedPropertyStore>().SetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, 2m);
            obj.As<IAttachedPropertyStore>().RemoveProperty(MyBindableObject.AttachedDecimalProperty.Identifier);

            AssertDefaultValue(obj, 1m);
        }

        private static void AssertDefaultValue (MyBindableObject obj, decimal expectedValue)
        {
            object value;

            obj.HasValue(MyBindableObject.AttachedDecimalProperty).Should().BeFalse();
            obj.GetValue(MyBindableObject.AttachedDecimalProperty).Should().Be(expectedValue);
            obj[MyBindableObject.AttachedDecimalProperty].Should().Be(expectedValue);
            obj.As<IAttachedPropertyStore>().PropertyCount.Should().Be(0);
            obj.As<IAttachedPropertyStore>().TryGetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, out value);
            value.Should().Be(expectedValue);
        }

        private static void AssertCustomValue (MyBindableObject obj, decimal expectedValue)
        {
            object value;
            var props = new KeyValuePair<AttachableMemberIdentifier, object>[1];

            obj.HasValue(MyBindableObject.AttachedDecimalProperty).Should().BeTrue();
            obj.GetValue(MyBindableObject.AttachedDecimalProperty).Should().Be(expectedValue);
            obj[MyBindableObject.AttachedDecimalProperty].Should().Be(expectedValue);
            obj.As<IAttachedPropertyStore>().PropertyCount.Should().Be(1);
            obj.As<IAttachedPropertyStore>().TryGetProperty(MyBindableObject.AttachedDecimalProperty.Identifier, out value);
            value.Should().Be(expectedValue);
            obj.As<IAttachedPropertyStore>().CopyPropertiesTo(props, 0);
            props[0].Should().Be(new KeyValuePair<AttachableMemberIdentifier, object>(MyBindableObject.AttachedDecimalProperty.Identifier, expectedValue));
        }

        [UsedImplicitly (ImplicitUseTargetFlags.WithMembers)]
        private class MyBindableObject : BindableObject
        {
            public static readonly AttachedProperty<decimal> AttachedDecimalProperty =
                AttachedProperty.Register<MyBindableObject, decimal>(nameof(AttachedDecimalProperty), 1m);

            public string MyStringValue { get; set; }
            public int MyInt32Value { get; set; }
        }

        public class Data
        {
            public string StringValue { get; set; }
            public int Int32Value { get; set; }
        }
    }
}