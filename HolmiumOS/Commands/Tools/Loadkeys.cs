using System;
using Cosmos.System.ScanMaps;
using HolmiumOS.Commands;
using Sys = Cosmos.System;

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
                    Sys.KeyboardManager.SetKeyLayout(new TRStandardLayout());
                    break;

                case "us":
                    Sys.KeyboardManager.SetKeyLayout(new USStandardLayout());
                    break;

                case "gb":
                    Sys.KeyboardManager.SetKeyLayout(new GBStandardLayout());
                    break;

                case "fr":
                    Sys.KeyboardManager.SetKeyLayout(new FRStandardLayout());
                    break;

                case "de":
                    Sys.KeyboardManager.SetKeyLayout(new DEStandardLayout());
                    break;

                case "es":
                    Sys.KeyboardManager.SetKeyLayout(new ESStandardLayout());
                    break;

                default:
                    Console.WriteLine("Kullanım: loadkeys <tr|us|gb|fr|de|es>");
                    return;
            }

            Console.WriteLine("Klavye duzeni yuklendi.");
        }
    }
}