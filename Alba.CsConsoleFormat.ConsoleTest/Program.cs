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
                Guid = Guid.NewGuid(),
                Date = DateTime.Now,
                Items = new List<DataItem> {
                    new DataItem { Id = 1, Name = "Name 1", Value = "Value 1" },
                    new DataItem { Id = 2, Name = "Name 2", Value = "Value 2" },
                }
            });
            //Console.WriteLine(((Span)((Para)doc.Children[0]).Children[0]).Text);
            //Console.WriteLine(((Span)((Para)doc.Children[1]).Children[0]).Text);
            Console.WriteLine(doc);

            var buffer = new ConsoleRenderBuffer { LineCharRenderer = LineCharRenderer.Box };
            var rainbow = new[] {
                ConsoleColor.Black,
                ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkBlue, ConsoleColor.DarkMagenta, ConsoleColor.DarkRed,
                ConsoleColor.Black,
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Red,
            };
            /*for (int i = 0; i < 16; i++)
                buffer.FillRectangle((ConsoleColor)i, i, i, 80 - i * 2, 31 - i * 2);*/
            for (int i = 0; i < rainbow.Length; i++)
                buffer.FillRectangle(rainbow[i], i, i, 80 - i * 2, (rainbow.Length - i) * 2);
            buffer.DrawHorizontalLine(ConsoleColor.White, 1, 0, 79);
            buffer.DrawHorizontalLine(ConsoleColor.White, 1, 1, 79, LineWidth.Wide);
            buffer.DrawHorizontalLine(ConsoleColor.White, 3, 3, 10);
            buffer.DrawVerticalLine(ConsoleColor.White, 1, 1, 10);
            buffer.DrawVerticalLine(ConsoleColor.White, 2, 2, 6);
            buffer.DrawVerticalLine(ConsoleColor.White, 5, 0, 6, LineWidth.Wide);
            buffer.DrawVerticalLine(ConsoleColor.White, 5, 0, 6);
            buffer.DrawVerticalLine(ConsoleColor.White, 6, 0, 6);
            buffer.DrawVerticalLine(ConsoleColor.White, 3, 0, 12, LineWidth.Wide);
            buffer.DrawRectangle(ConsoleColor.White, 0, 0, 80, rainbow.Length * 2, LineWidth.Wide);
            buffer.FillVerticalLine(ConsoleColor.Yellow, 40, 0, rainbow.Length * 2);
            buffer.DrawString(ConsoleColor.Black, 15, 15, "Hello world!");
            buffer.RenderToConsole();
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
        public Guid Guid { get; set; }
        public DateTime Date { get; set; }
        public List<DataItem> Items { get; set; }

        public override string ToString ()
        {
            return "Data";
        }
    }

    internal class DataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString ()
        {
            return "DataItem";
        }
    }
}