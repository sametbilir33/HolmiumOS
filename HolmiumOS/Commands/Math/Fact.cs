using System;

namespace HolmiumOS.Commands.Math
{
    public class Fact : ICommand
    {
        public string Name => "fact";
        public string Description => "Bir sayinin faktoriyelini hesaplar";
        public string Usage => "fact <sayi>";

        public void Execute(string args)
        {
            if (!int.TryParse(args, out int number) || number < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gecersiz sayi");
                Console.ResetColor();
                return;
            }

            long factorial = 1;
            for (int i = 2; i <= number; i++)
                factorial *= i;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{number}! = {factorial}");
            Console.ResetColor();
        }
    }
}