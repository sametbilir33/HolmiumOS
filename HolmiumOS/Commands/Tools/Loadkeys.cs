using System;
using Cosmos.Kernel.System.Input;
using Cosmos.Kernel.System.Input.Layouts;

namespace HolmiumOS.Commands.Tools
{
    public class LoadKeys : ICommand
    {
        public string Name => "loadkeys";
        public string Description => "Klavye duzenini yukler.";
        public string Usage => "loadkeys <tr|us|gb|fr|de|es>";

        public void Execute(string args)
        {
            switch (args.ToLower())
            {
                case "tr":
                    KeyboardManager.SetLayout(new TRStandardLayout());
                    break;

                case "us":
                    KeyboardManager.SetLayout(new USStandardLayout());
                    break;

                case "gb":
                    KeyboardManager.SetLayout(new GBStandardLayout());
                    break;

                case "fr":
                    KeyboardManager.SetLayout(new FRStandardLayout());
                    break;

                case "de":
                    KeyboardManager.SetLayout(new DEStandardLayout());
                    break;

                case "es":
                    KeyboardManager.SetLayout(new ESStandardLayout());
                    break;

                default:
                    Console.WriteLine("Kullanım: loadkeys <tr|us|gb|fr|de|es>");
                    return;
            }

            Console.WriteLine("Klavye duzeni yuklendi.");
        }
    }
}