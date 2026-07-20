using System;

namespace HolmiumOS.Commands.System
{
    public class Clear : ICommand
    {
        public string Name => "clear";
        public string Description => "Ekrani temizler";
        public string Usage => "clear";

        public void Execute(string args)
        {
            Console.Clear();
        }
    }
}