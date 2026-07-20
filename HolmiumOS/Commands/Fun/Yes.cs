using System;

namespace HolmiumOS.Commands.Fun
{
    public class Yes : ICommand
    {
        public string Name => "yes";
        public string Description => "Metni sonsuza kadar tekrarla";
        public string Usage => "yes";

        public void Execute(string args)
        {
            string text = "y";

            if (!string.IsNullOrWhiteSpace(args))
                text = args;


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                        break;
                }

                Console.WriteLine(text);
            }
        }
    }
}