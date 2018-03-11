using System.Collections;
using System.ComponentModel;

namespace Alba.CsConsoleFormat
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ValuesInitializer : IEnumerable
    {
        public BindableObject Object { get; }

        public ValuesInitializer(BindableObject obj)
        {
            Object = obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }
    }
}