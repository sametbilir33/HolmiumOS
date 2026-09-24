using System;
using System.Numerics;

namespace HolmiumOS.Commands.Math
{
    public class Pow : ICommand
    {
        public string Name => "pow";
        public string Description => "Bir sayinin ussunu alir";
        public string Usage => "pow <a> <b>";

        public void Execute(string args)
        {
            string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: pow <a> <b>");
                Console.ResetColor();
                return;
            }

            if (!BigInteger.TryParse(parts[0], out BigInteger baseNum) ||
                !int.TryParse(parts[1], out int exponent))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gecersiz sayi");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;

            if (exponent >= 0)
            {
                BigInteger result = BigIntPow(baseNum, exponent);
                Console.WriteLine($"{baseNum}^{exponent} = {result}");
            }
            else
            {
                BigInteger positive = BigIntPow(baseNum, -exponent);

                if (positive == 0)
                {
                    Console.WriteLine("0 ile bolme hatasi");
                }
                else
                {
                    // kesirli sonuç (1 / n)
                    Console.WriteLine($"{baseNum}^{exponent} = 1/{positive}");
                }
            }

            Console.ResetColor();
        }

        private BigInteger BigIntPow(BigInteger value, int exp)
        {
            BigInteger result = 1;

            for (int i = 0; i < exp; i++)
                result *= value;

            return result;
        }
    }
}