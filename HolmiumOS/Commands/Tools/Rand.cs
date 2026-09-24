using System;

namespace HolmiumOS.Commands.Tools
{
    public class Rand : ICommand
    {
        public string Name => "rand";
        public string Description => "Belirtilen aralikta rastgele sayi uretir";
        public string Usage => "rand <min> <max>";

        public void Execute(string args)
        {
            string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: rand <min> <max>");
                Console.ResetColor();
                return;
            }

            if (!long.TryParse(parts[0], out long min) || !long.TryParse(parts[1], out long max))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hata: Lutfen sadece tam sayilar girin!");
                Console.ResetColor();
                return;
            }

            if (max < min)
            {
                long tmp = min;
                min = max;
                max = tmp;
            }

            Random rnd = new();
            long value = (long)(rnd.NextDouble() * (max - min + 1)) + min;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Rastgele sayi: {value}");
            Console.ResetColor();
        }
    }
}