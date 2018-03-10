using System.Collections;

namespace Alba.CsConsoleFormat
{
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