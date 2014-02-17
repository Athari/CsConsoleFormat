using System;

namespace Alba.CsConsoleFormat.ConsoleTest
{
    internal class Program
    {
        private static void Main ()
        {
            var body = new ConBody(
                new ConBackgroundAttr(ConsoleColor.DarkBlue),
                new ConForegroundAttr(ConsoleColor.Yellow),
                " abc ",
                new ConP(
                    new ConBackgroundAttr(ConsoleColor.DarkMagenta),
                    " ",
                    new ConP(new ConForegroundAttr(ConsoleColor.Red), "d"),
                    new ConP(new ConForegroundAttr(ConsoleColor.Green), "e"),
                    new ConP(new ConForegroundAttr(ConsoleColor.Blue), "f"),
                    " "
                    ),
                new ConP(
                    new ConBackgroundAttr(ConsoleColor.DarkGray),
                    new ConForegroundAttr(ConsoleColor.White),
                    " ghi "
                    ),
                " zxc \n"
                );
            Console.WriteLine(body);
            new ConsoleFormatter().Write(body);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}