using System;

namespace HolmiumOS.Commands.Math
{
    public class Mult : ICommand
    {
        public string Name => "mult";
        public string Description => "Girilen sayidan baslayarak her satirda iki katini yazar";
        public string Usage => "mult <sayi>";

        public void Execute(string args)
        {
            if (!long.TryParse(args, out long baseNum))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gecersiz sayi");
                Console.ResetColor();
                return;
            }

            long current = baseNum;
            Console.Clear();
            Console.WriteLine("ESC = cik | ENTER = devam\n");

            while (true)
            {
                int lines = Console.WindowHeight - 3;
                for (int i = 0; i < lines; i++)
                {
                    Console.WriteLine(current);
                    current *= 2;
                }

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                    break;

                Console.Clear();
            }
        }
    }
}