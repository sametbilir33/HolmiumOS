using System;
using System.Linq;
using HolmiumOS.Commands;

namespace HolmiumOS.Commands.System
{
    public class Pause : ICommand
    {
        public string Name => "pause";
        public string Description => "Bir tusa basilmasini bekler, istege bagli mesaj gosterebilir";
        public string Usage => "pause [mesaj]";

        public void Execute(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                Console.Clear();
                Console.WriteLine(args);
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Devam etmek icin bir tusa basin...");
                Console.ReadKey(true);
            }
        }
    }
}