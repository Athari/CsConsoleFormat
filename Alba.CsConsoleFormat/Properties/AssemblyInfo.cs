using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: Guid("4ad934e0-78da-4356-917f-130742476086")]

[assembly: XmlnsPrefix("urn:alba:cs-console-format", "a")]
[assembly: XmlnsDefinition("urn:alba:cs-console-format", "Alba.CsConsoleFormat")]
[assembly: XmlnsDefinition("urn:alba:cs-console-format", "Alba.CsConsoleFormat.Markup")]

[assembly: InternalsVisibleTo("Alba.CsConsoleFormat.Presentation")]
[assembly: InternalsVisibleTo("Alba.CsConsoleFormat.Tests")]
