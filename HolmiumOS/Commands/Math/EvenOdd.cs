using System;
using HolmiumOS.Commands;

namespace HolmiumOS.Commands.Math
{
    public class EvenOdd : ICommand
    {
        public string Name => "evenodd";
        public string Description => "Girilen sayinin tek mi cift mi oldugunu gosterir";
        public string Usage => "evenodd <sayi>";

        public void Execute(string args)
        {
            if (!long.TryParse(args, out long eo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gecersiz sayi");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(eo % 2 == 0 ? "cift" : "Tek");
            Console.ResetColor();
        }
    }
}