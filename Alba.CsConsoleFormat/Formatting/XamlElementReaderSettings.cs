using System;
using System.Collections.Generic;
using System.Reflection;

namespace Alba.CsConsoleFormat
{
    public sealed class XamlElementReaderSettings
    {
        public bool CloseInput { get; set; }
        public IList<Assembly> ReferenceAssemblies { get; }

        public XamlElementReaderSettings()
        {
            ReferenceAssemblies = new List<Assembly> {
                typeof(Document).Assembly,
                typeof(Console).Assembly,
            };
        }
    }
}