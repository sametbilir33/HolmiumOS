using System;

namespace HolmiumOS.Commands.Math
{
    public class IsPrime : ICommand
    {
        public string Name => "isprime";
        public string Description => "Girilen sayinin asal olup olmadigini kontrol eder";
        public string Usage => "isprime <sayi>";

        public void Execute(string args)
        {
            if (!int.TryParse(args, out int n) || n < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Asal degil");
                Console.ResetColor();
                return;
            }

            bool prime = true;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    prime = false;
                    break;
                }
            }

            Console.ForegroundColor = prime ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(prime ? "Asal sayi" : "Asal degil");
            Console.ResetColor();
        }
    }
}