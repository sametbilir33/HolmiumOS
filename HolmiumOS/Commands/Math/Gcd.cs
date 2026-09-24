using System;
using SysMath = System.Math;

namespace HolmiumOS.Commands.Math
{
    public class Gcd : ICommand
    {
        public string Name => "gcd";
        public string Description => "Iki sayinin EBOB'unu (En Buyuk Ortak Bolen) hesaplar";
        public string Usage => "gcd <a> <b>";

        public void Execute(string args)
        {
            string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !long.TryParse(parts[0], out long a) ||
                !long.TryParse(parts[1], out long b))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: gcd <a> <b>");
                Console.ResetColor();
                return;
            }

            long ComputeGcd(long x, long y)
            {
                while (y != 0)
                {
                    long temp = y;
                    y = x % y;
                    x = temp;
                }
                return SysMath.Abs(x);
            }

            long result = ComputeGcd(a, b);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"EBOB({a}, {b}) = {result}");
            Console.ResetColor();
        }
    }
}
