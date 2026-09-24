using System;
using SysMath = System.Math;

namespace HolmiumOS.Commands.Math
{
    public class Sqrt : ICommand
    {
        public string Name => "sqrt";
        public string Description => "Bir sayinin karekokunu hesaplar";
        public string Usage => "sqrt <sayi>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: sqrt <sayi>");
                Console.ResetColor();
                return;
            }

            try
            {
                if (!double.TryParse(args, out double number))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hata: Gecersiz sayi");
                    Console.ResetColor();
                    return;
                }

                if (number < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hata: Negatif sayilarin karekoku alinamaz");
                    Console.ResetColor();
                    return;
                }

                double sqrt = SysMath.Sqrt(number);
                int sqrtInt = (int)SysMath.Round(sqrt);

                Console.ForegroundColor = ConsoleColor.Green;

                if (SysMath.Abs(sqrtInt * sqrtInt - number) < 0.000001)
                {
                    Console.WriteLine($"Karekok: {sqrtInt} (tam kare)");
                }
                else
                {
                    Console.WriteLine($"Karekok: {sqrt:F5} (yaklasik)");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {e.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}