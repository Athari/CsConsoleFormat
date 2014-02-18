using System;

namespace Alba.CsConsoleFormat.ConsoleTest
{
    internal class Program
    {
        private static void Main ()
        {
            var body = new ConBody(
                ConBackgroundAttr.DarkBlue, ConForegroundAttr.Yellow,
                " abc ",
                new ConPara(
                    ConBackgroundAttr.DarkMagenta,
                    " ",
                    new ConPara(ConForegroundAttr.Red, "d"),
                    new ConPara(ConForegroundAttr.Green, "e"),
                    new ConPara(ConForegroundAttr.Blue, "f"),
                    " "
                    ),
                new ConPara(
                    ConBackgroundAttr.DarkGray, ConForegroundAttr.White,
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