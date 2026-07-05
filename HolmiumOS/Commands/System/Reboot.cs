using System;

namespace HolmiumOS.Commands.System
{
    public class Reboot : ICommand
    {
        public string Name => "reboot";
        public string Description => "Sistemi yeniden baslatir";
        public string Usage => "reboot";

        public void Execute(string args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sistem yeniden baslatiliyor...");
            Console.ResetColor();
            Cosmos.System.Power.Reboot();
        }
    }
}