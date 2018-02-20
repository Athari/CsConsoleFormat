using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Xaml;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    using IntPropertyExpression = Expression<Func<AttachedProperty<int>>>;

    public sealed class AttachedPropertyTests
    {
        //private int My { get; set; }
        private static AttachedProperty<int> MyIntProperty { get; } = null;
        private static AttachedProperty<char> MyChar { get; } = null;
        private static AttachedProperty<string> MyStringProperty { get; } = null;

        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        [SuppressMessage("ReSharper", "LocalNameCapturedOnly")]
        public void NullArguments()
        {
            var name = "MyPropertyCreateNullArguments";
            var nameExpression = (IntPropertyExpression)(() => MyIntProperty);
            var identifier = new AttachableMemberIdentifier(typeof(AttachedPropertyTests), name);

            new Action(() => _ = AttachedProperty.Register<AttachedPropertyTests, int>((string)null, 1)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(name));
            new Action(() => _ = AttachedProperty.Register<AttachedPropertyTests, int>((IntPropertyExpression)null, 1)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(nameExpression));
            new Action(() => _ = AttachedProperty.Get(null)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be(nameof(identifier));
        }

        [Fact]
        public void Create()
        {
            var identifier = new AttachableMemberIdentifier(typeof(AttachedPropertyTests), "MyPropertyCreate");
            var property = new AttachedProperty(identifier, "DefaultValue");
            property.DefaultValueUntyped.Should().Be("DefaultValue");
            property.Identifier.Should().Be(identifier);
            property.Name.Should().Be("MyPropertyCreate");
            property.OwnerType.Should().Be<AttachedPropertyTests>();
        }

        [Fact]
        public void CreateGeneric()
        {
            var identifier = new AttachableMemberIdentifier(typeof(AttachedPropertyTests), "MyPropertyCreateGeneric");
            var property = new AttachedProperty<string>(identifier, "DefaultValue");
            property.DefaultValue.Should().Be("DefaultValue");
            property.DefaultValueUntyped.Should().Be("DefaultValue");
            property.Identifier.Should().Be(identifier);
            property.Name.Should().Be("MyPropertyCreateGeneric");
            property.OwnerType.Should().Be<AttachedPropertyTests>();
        }

        [Fact]
        public void RegisterAttached()
        {
            AttachedProperty<int> property = AttachedProperty.Register<AttachedPropertyTests, int>("MyPropertyRegisterAttached", 9);
            property.DefaultValue.Should().Be(9);
            property.DefaultValueUntyped.Should().Be(9);
            property.Name.Should().Be("MyPropertyRegisterAttached");
            property.OwnerType.Should().Be<AttachedPropertyTests>();
        }

        [Fact]
        public void RegisterDouble()
        {
            const string Name = "MyPropertyRegisterDouble";
            _ = AttachedProperty.Register<AttachedPropertyTests, int>(Name);
            new Action(() => _ = AttachedProperty.Register<AttachedPropertyTests, int>(Name)).ShouldThrow<ArgumentException>()
                .WithMessage("*already registered*").WithMessage($"*{typeof(AttachedPropertyTests).Name}*").WithMessage($"*{Name}*");
        }

        [Fact]
        public void RegisterWithSuffix()
        {
            AttachedProperty<string> property = AttachedProperty.Register<AttachedPropertyTests, string>(() => MyStringProperty, "8");
            property.DefaultValue.Should().Be("8");
            property.DefaultValueUntyped.Should().Be("8");
            property.Name.Should().Be("MyString");
            property.OwnerType.Should().Be<AttachedPropertyTests>();
        }

        [Fact]
        public void RegisterWithoutSuffix()
        {
            AttachedProperty<char> property = AttachedProperty.Register<AttachedPropertyTests, char>(() => MyChar, '7');
            property.DefaultValue.Should().Be('7');
            property.DefaultValueUntyped.Should().Be('7');
            property.Name.Should().Be(nameof(MyChar));
            property.OwnerType.Should().Be<AttachedPropertyTests>();
        }
    }
}