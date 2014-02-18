using System;
using System.Collections.Generic;
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
            var doc = ReadXaml<Document>(new Data {
                Title = "Header Title",
                SubTitle = "Header SubTitle",
            });
            Console.WriteLine(((Span)((Para)doc.Children[0]).Children[0]).Text);
            Console.WriteLine(((Span)((Para)doc.Children[1]).Children[0]).Text);
            Console.WriteLine(doc);
        }

        private T ReadXaml<T> (object dataContext) where T : Element, new()
        {
            using (Stream resStream = GetType().Assembly.GetManifestResourceStream(GetType(), "Markup.xaml")) {
                //return (Document)XamlServices.Load(resStream);
                int pad = 0;
                var objects = new Stack<object>();
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
                    RootObjectInstance = new T { DataContext = dataContext },
                    AfterBeginInitHandler = (sender, args) => {
                        objects.Push(args.Instance);
                        /*var element = args.Instance as Element;
                        if (element != null) {
                            
                        }*/
                        Console.WriteLine(new string(' ', pad++ * 2) + "<{0}>", args.Instance);
                    },
                    AfterEndInitHandler = (sender, args) => {
                        objects.Pop();
                        Console.WriteLine(new string(' ', --pad * 2) + "</{0}>", args.Instance);
                    },
                };
                using (var xamlReader = new XamlXmlReader(resStream, context, readerSettings))
                using (var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings)) {
                    XamlServices.Transform(xamlReader, xamlWriter, false);
                    return (T)xamlWriter.Result;
                }
            }
        }
    }

    internal class Data
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
}