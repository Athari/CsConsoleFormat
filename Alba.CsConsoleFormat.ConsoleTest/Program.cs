using System;
using System.IO;
using System.Xaml;

namespace Alba.CsConsoleFormat.ConsoleTest
{
    internal class Program
    {
        public static void Main ()
        {
            new Program().Run();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private void Run ()
        {
            Document doc = ReadDocument();
            Console.WriteLine(doc);
        }

        private Document ReadDocument ()
        {
            using (Stream resStream = GetType().Assembly.GetManifestResourceStream(GetType(), "Markup.xaml")) {
                //return (Document)XamlServices.Load(resStream);
                int pad = 0;
                var context = new XamlSchemaContext(new[] {
                    //GetType().Assembly,
                    typeof(Document).Assembly,
                }, new XamlSchemaContextSettings {
                    SupportMarkupExtensionsWithDuplicateArity = true,
                });
                var readerSettings = new XamlXmlReaderSettings {
                    ProvideLineInfo = true,
                };
                var writerSettings = new XamlObjectWriterSettings {
                    AfterBeginInitHandler = (sender, args) => Console.WriteLine(new string(' ', pad++ * 2) + "<{0}>", args.Instance),
                    AfterEndInitHandler = (sender, args) => Console.WriteLine(new string(' ', --pad * 2) + "</{0}>", args.Instance),
                };
                using (var xamlReader = new XamlXmlReader(resStream, context, readerSettings))
                using (var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings)) {
                    XamlServices.Transform(xamlReader, xamlWriter, false);
                    return (Document)xamlWriter.Result;
                }
            }
        }
    }
}