using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public struct AttachedValue<T>
    {
        public AttachedProperty<T> Property { get; }
        public T Value { get; }

        public AttachedValue(AttachedProperty<T> property, T value)
        {
            Property = property;
            Value = value;
        }
    }
}