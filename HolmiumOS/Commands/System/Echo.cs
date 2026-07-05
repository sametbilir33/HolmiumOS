using System;
using HolmiumOS.Commands;

namespace HolmiumOS.Commands.System
{
    public class Echo : ICommand
    {
        public string Name => "echo";
        public string Description => "Girilen metni ekrana yazdirir";
        public string Usage => "echo <metin>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: echo <metin>");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(args);
            }

            Console.ResetColor();
        }
    }
}