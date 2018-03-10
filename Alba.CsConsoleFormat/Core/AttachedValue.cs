namespace Alba.CsConsoleFormat
{
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