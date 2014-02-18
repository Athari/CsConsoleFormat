﻿using System;
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
                Items = new List<DataItem> {
                    new DataItem { Id = "1", Name = "Name 1", Value = "Value 1" },
                    new DataItem { Id = "2", Name = "Name 2", Value = "Value 2" },
                }
            });
            //Console.WriteLine(((Span)((Para)doc.Children[0]).Children[0]).Text);
            //Console.WriteLine(((Span)((Para)doc.Children[1]).Children[0]).Text);
            Console.WriteLine(doc);
        }

        private T ReadXaml<T> (object dataContext) where T : Element, new()
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
                    RootObjectInstance = new T { DataContext = dataContext },
                    AfterBeginInitHandler = (sender, args) => Console.WriteLine(new string(' ', pad++ * 2) + "<{0}>", args.Instance),
                    AfterEndInitHandler = (sender, args) => Console.WriteLine(new string(' ', --pad * 2) + "</{0}>", args.Instance),
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
        public List<DataItem> Items { get; set; }
    }

    internal class DataItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}