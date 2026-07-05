using System;

namespace HolmiumOS.Commands.System
{
    public class Shutdown : ICommand
    {
        public string Name => "shutdown";
        public string Description => "Sistemi kapatir";
        public string Usage => "shutdown";

        public void Execute(string args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sistem kapatiliyor...");
            Console.ResetColor();
            Cosmos.System.Power.Shutdown();
        }
    }
}