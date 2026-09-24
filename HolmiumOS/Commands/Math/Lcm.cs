using System;
using SysMath = System.Math;

namespace HolmiumOS.Commands.Math
{
    public class Lcm : ICommand
    {
        public string Name => "lcm";
        public string Description => "Iki sayinin EKOK'unu (En Kucuk Ortak Kat) hesaplar";
        public string Usage => "lcm <a> <b>";

        public void Execute(string args)
        {
            string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !long.TryParse(parts[0], out long a) ||
                !long.TryParse(parts[1], out long b))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: lcm <a> <b>");
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

            long result = SysMath.Abs(a * b) / ComputeGcd(a, b);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"EKOK({a}, {b}) = {result}");
            Console.ResetColor();
        }
    }
}