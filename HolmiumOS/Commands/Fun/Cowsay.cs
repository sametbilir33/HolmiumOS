using System;

namespace HolmiumOS.Commands.Fun
{
    public class Cowsay : ICommand
    {
        public string Name => "cowsay";
        public string Description => "Inek seklinde mesaj gosterir (max 40 karakter)";
        public string Usage => "cowsay <metin>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: cowsay <metin>");
                Console.ResetColor();
                return;
            }

            if (args.Length > 40)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hata: Mesaj 40 karakterden uzun olamaz!");
                Console.ResetColor();
                return;
            }

            int balloonWidth = args.Length;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" {new string('-', balloonWidth + 2)}");
            Console.WriteLine($"< {args} >");
            Console.WriteLine($" {new string('-', balloonWidth + 2)}");
            Console.WriteLine("        \\   ^__^");
            Console.WriteLine("         \\  (oo)\\_______");
            Console.WriteLine("            (__)\\       )\\/");
            Console.WriteLine("                ||----w |");
            Console.WriteLine("                ||     ||");
            Console.ResetColor();
        }
    }
}