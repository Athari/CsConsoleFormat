using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace
// ReSharper disable AnnotateCanBeNullTypeMember
namespace System
{
    /// <summary>Trivial Lazy without thread-safety. Exceptions are not compatible.</summary>
    [Serializable]
    [DebuggerTypeProxy(typeof(System_LazyDebugView<>))]
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
    internal class Lazy<T>
    {
        private object m_boxed;

        [NonSerialized]
        private Func<T> m_valueFactory;

        private static readonly Func<T> AlreadyInvoked = () => throw new InvalidOperationException();

        public Lazy([NotNull] Func<T> valueFactory)
        {
            m_valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        internal T ValueForDebugDisplay => IsValueCreated ? ((ValueHolder)m_boxed)._value : default;
        internal bool IsValueFaulted => m_boxed is ExceptionHolder;
        public bool IsValueCreated => m_boxed is ValueHolder;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                if (m_boxed == null)
                    m_boxed = CreateValue();
                switch (m_boxed) {
                    case ValueHolder value:
                        return value._value;
                    case ExceptionHolder ex:
                        throw new TargetInvocationException("Lazy faulted.", ex._exception);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private object CreateValue()
        {
            try {
                if (m_valueFactory != null) {
                    if (m_valueFactory == AlreadyInvoked)
                        throw new InvalidOperationException("Recursive call to Lazy.Value");
                    Func<T> valueFactory = m_valueFactory;
                    m_valueFactory = AlreadyInvoked;
                    return new ValueHolder(valueFactory());
                }
                else {
                    try {
                        return new ValueHolder((T)Activator.CreateInstance(typeof(T)));
                    }
                    catch (MissingMethodException ex) {
                        throw new InvalidOperationException("No parameterless constructor.", ex);
                    }
                }
            }
            catch (Exception ex) {
                return new ExceptionHolder(ex);
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _ = Value;
        }

        public override string ToString()
        {
            return IsValueCreated ? Value.ToString() : "Value is not created.";
        }

        [Serializable]
        private sealed class ValueHolder
        {
            public readonly T _value;
            public ValueHolder(T value) => _value = value;
        }

        private sealed class ExceptionHolder
        {
            public readonly Exception _exception;
            public ExceptionHolder(Exception ex) => _exception = ex;
        }
    }

    internal sealed class System_LazyDebugView<T>
    {
        private readonly Lazy<T> _lazy;
        public System_LazyDebugView(Lazy<T> lazy) => _lazy = lazy;
        public bool IsValueCreated => _lazy.IsValueCreated;
        public bool IsValueFaulted => _lazy.IsValueFaulted;
        public T Value => _lazy.ValueForDebugDisplay;
    }
}