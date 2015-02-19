using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xaml;

namespace Alba.CsConsoleFormat.ConsoleTest
{
    internal class Program
    {
        public static void Main ()
        {
            new Program().Run();
            Console.WriteLine("Done!");
            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private void Run ()
        {
            Size consoleSize = Size.Min(ConsoleRenderer.ConsoleLargestWindowSize, new Size((25 + 1) * 7 + 1, 60));
            try {
                ConsoleRenderer.ConsoleWindowRect = new Rect(consoleSize);
                Console.BufferWidth = consoleSize.Width;
            }
            catch (Exception e) {
                var consoleRect = new Rect(Console.WindowLeft, Console.WindowTop, Console.WindowWidth, Console.WindowHeight);
                Console.WriteLine(consoleRect);
                var bufferRect = new Size(Console.BufferWidth, Console.BufferHeight);
                Console.WriteLine(bufferRect);
                Console.WriteLine(e.Message);
            }
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = Path.GetFileNameWithoutExtension(Console.Title);

            var data = new Data {
                Title = "Header Title",
                SubTitle = "Header SubTitle",
                Formatted = "Aaaa\nBbbb\nCccc",
                LoremIpsum = "Lo|rem ip|sum do|lor sit amet, con|sec|te|tur adi|pis|cing elit, sed do eius|mod tem|por in|ci|di|dunt ut la|bo|re et do|lo|re mag|na ali|qua. Ut enim ad mi|nim ve|ni|am, qu|is nos|trud exer|ci|ta|tion ul|lam|co la|bo|ris ni|si ut ali|quip ex ea com|mo|do con|se|quat. Du|is au|te iru|re do|lor in rep|re|hen|de|rit in vo|lup|ta|te ve|lit es|se cil|lum do|lo|re eu fu|gi|at nul|la pa|ri|a|tur. Ex|cep|te|ur sint oc|ca|e|cat cu|pi|da|tat non pro|i|dent, sunt in cul|pa qui of|fi|cia de|se|runt mol|lit anim id est la|bo|rum.",
                Guid = Guid.NewGuid(),
                Date = DateTime.Now,
                Items = new List<DataItem> {
                    new DataItem {
                        Id = 1, Name = "Name 1", Value = "Value 1",
                        SubItems = new List<DataItem> {
                            new DataItem { Id = 11, Name = "Name 1.1", Value = "Value 1.1" },
                            new DataItem { Id = 12, Name = "Name 1.2", Value = "Value 1.2" },
                        }
                    },
                    new DataItem { Id = 2, Name = "Name 2", Value = "Value 2" },
                }
            };

            /*if (MemoryProfiler.IsActive) {
                MemoryProfiler.EnableAllocations();
                MemoryProfiler.Dump();
            }
            new ConsoleRenderer().RenderDocument(ReadXaml<Document>(data));
            if (MemoryProfiler.IsActive)
                MemoryProfiler.Dump();*/
            /*for (int i = 0; i < 100; i++)
                new ConsoleRenderer().RenderDocument(ReadXaml<Document>(data));*/
            /*if (MemoryProfiler.IsActive)
                MemoryProfiler.Dump();*/

            var doc = ReadXaml<Document>(data);
            //Console.WriteLine(((Span)((Para)doc.Children[0]).Children[0]).Text);
            //Console.WriteLine(((Span)((Para)doc.Children[1]).Children[0]).Text);
            new ConsoleRenderer().RenderDocument(doc);

            var buffer = new ConsoleRenderBuffer {
                LineCharRenderer = LineCharRenderer.Box,
                //Clip = new Rect(1, 1, 78, 30),
            };
            var rainbow = new[] {
                ConsoleColor.Black,
                ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkBlue, ConsoleColor.DarkMagenta, ConsoleColor.DarkRed,
                ConsoleColor.Black,
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Red,
            };
            /*for (int i = 0; i < 16; i++)
                buffer.FillRectangle((ConsoleColor)i, i, i, 80 - i * 2, 31 - i * 2);*/
            for (int i = 0; i < rainbow.Length; i++)
                buffer.FillBackgroundRectangle(i, i, 80 - i * 2, (rainbow.Length - i) * 2, rainbow[i]);
            buffer.DrawHorizontalLine(0, 1, 79, ConsoleColor.White);
            buffer.DrawHorizontalLine(1, 1, 79, ConsoleColor.White, LineWidth.Wide);
            buffer.DrawHorizontalLine(3, 3, 10, ConsoleColor.White);
            buffer.DrawVerticalLine(1, 1, 10, ConsoleColor.White);
            buffer.DrawVerticalLine(2, 2, 6, ConsoleColor.White);
            buffer.DrawVerticalLine(5, 0, 6, ConsoleColor.White, LineWidth.Wide);
            buffer.DrawVerticalLine(5, 0, 6, ConsoleColor.White);
            buffer.DrawVerticalLine(6, 0, 6, ConsoleColor.White);
            buffer.DrawVerticalLine(3, 0, 12, ConsoleColor.White, LineWidth.Wide);
            buffer.DrawRectangle(0, 0, 80, 32, ConsoleColor.White, LineWidth.Wide);
            buffer.FillBackgroundVerticalLine(40, 0, 32, ConsoleColor.Yellow);
            buffer.FillForegroundVerticalLine(41, 0, 32, ConsoleColor.White, Chars.FullBlock);
            buffer.FillForegroundVerticalLine(42, 0, 32, ConsoleColor.White, Chars.DarkShade);
            buffer.FillForegroundVerticalLine(43, 0, 32, ConsoleColor.White, Chars.MediumShade);
            buffer.FillForegroundVerticalLine(44, 0, 32, ConsoleColor.White, Chars.LightShade);
            buffer.DrawString(15, 15, ConsoleColor.Black, "Hello world!");
            buffer.DrawString(15, 16, ConsoleColor.White, "Hello world! Hello world! Hello world! Hello world! Hello world! Hello world!");
            //buffer.ApplyBackgroundColorMap(0, 0, buffer.Width, buffer.Height, ColorMaps.Invert);
            //buffer.ApplyForegroundColorMap(0, 0, buffer.Width, buffer.Height, ColorMaps.Invert);
            //buffer.RenderToConsole();

            /*Console.WriteLine(Console.OutputEncoding);
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("■▬▲►▼◄");
            Console.WriteLine("▀▄█▌▐");
            Console.WriteLine("♠♣♥♦");
            Console.WriteLine("☺☻☼♀♂♫");
            Console.WriteLine("«»‘’‚‛“”„‟‹›");*/
            /*foreach (EncodingInfo encoding in Encoding.GetEncodings()) {
                try {
                    Console.OutputEncoding = encoding.GetEncoding();
                    Console.WriteLine("{0,-10}{1,-20}«»‘’‚‛“”„‟‹›", encoding.CodePage, encoding.Name);
                }
                catch {
                    Console.WriteLine("{0,-10}{1,-20}FAILED", encoding.CodePage, encoding.Name);
                }
            }*/
            /*for (int i = 1; i < 10000; i += 10) {
                var sb = new StringBuilder();
                for (int j = 0; j < 10; j++)
                    sb.AppendFormat("{0,-6}{1} ", (i + j), (char)(i + j));
                Console.Write(sb);
            }*/
        }

        private T ReadXaml<T> (object dataContext) where T : Element, new()
        {
            using (Stream resStream = GetType().Assembly.GetManifestResourceStream(GetType(), "Markup.xaml")) {
                if (resStream == null)
                    throw new FileNotFoundException("Resource not found.");
                //return (T)XamlServices.Load(resStream);
                //int pad = 1;
                var context = new XamlSchemaContext(new[] {
                    typeof(Document).Assembly,
                    typeof(Console).Assembly,
                    typeof(Program).Assembly,
                }, new XamlSchemaContextSettings {
                    SupportMarkupExtensionsWithDuplicateArity = true,
                });
                var readerSettings = new XamlXmlReaderSettings {
                    ProvideLineInfo = true,
                };
                var writerSettings = new XamlObjectWriterSettings {
                    RootObjectInstance = new T { DataContext = dataContext },
                    //AfterBeginInitHandler = (sender, args) => Console.WriteLine(new string(' ', pad++ * 2) + "<{0}>", args.Instance),
                    //AfterEndInitHandler = (sender, args) => Console.WriteLine(new string(' ', --pad * 2) + "</{0}>", args.Instance),
                };
                using (var xamlReader = new XamlXmlReader(resStream, context, readerSettings))
                using (var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings)) {
                    XamlServices.Transform(xamlReader, xamlWriter, false);
                    return (T)xamlWriter.Result;
                }
            }
        }
    }

    public class Data
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Formatted { get; set; }
        public string LoremIpsum { get; set; }
        public Guid Guid { get; set; }
        public DateTime Date { get; set; }
        public List<DataItem> Items { get; set; }

        public static string Replace (string value, string to)
        {
            return value.Replace("|", to);
        }

        public static string Replace (string value, char to)
        {
            return value.Replace('|', to);
        }

        public static IEnumerable<string> ReplaceSplit (string value, string to)
        {
            return Split(Replace(value, to));
        }

        public static IEnumerable<string> ReplaceSplit (string value, char to)
        {
            return Split(Replace(value, to));
        }

        private static IEnumerable<string> Split (string value)
        {
            return value.Select(c => c.ToString());
        }

        public override string ToString ()
        {
            return "Data";
        }
    }

    public class DataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<DataItem> SubItems { get; set; }

        public override string ToString ()
        {
            return "DataItem";
        }
    }
}