using System;
using System.Collections.Generic;
using System.Linq;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands
{
    public class Help : ICommand
    {
        public string Name => "help";
        public string Description => "Komutlari listeler veya detay gosterir";
        public string Usage => "help [komut]";

        public void Execute(string args)
        {
            var allCommands = new List<ICommand>(CommandManager.Commands);

            Console.Clear();

            if (!string.IsNullOrWhiteSpace(args))
            {
                var cmd = allCommands.FirstOrDefault(c =>
                    c.Name.Equals(args.Trim(), StringComparison.OrdinalIgnoreCase));

                if (cmd == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Komut bulunamadi.");
                    Console.ResetColor();
                    Console.ReadKey(true);
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Komut: {cmd.Name}\n");
                Console.ResetColor();

                Console.WriteLine($"Aciklama: {cmd.Description}");
                Console.WriteLine($"Kullanim: {cmd.Usage}");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\nDevam etmek icin herhangi bir tusa bas");
                Console.ResetColor();

                Console.ReadKey(true);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Mevcut Komutlar\n");
            Console.ResetColor();

            string commandList = string.Join(", ", allCommands.Select(c => c.Name));
            WriteWrapped(commandList, Console.WindowWidth - 1);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nDevam etmek icin herhangi bir tusa bas");
            Console.ResetColor();

            Console.ReadKey(true);
        }

        private void WriteWrapped(string text, int width)
        {
            var words = text.Split(' ');
            var line = "";

            foreach (var word in words)
            {
                if ((line + word).Length >= width)
                {
                    Console.WriteLine(line.TrimEnd());
                    line = word + " ";
                }
                else
                {
                    line += word + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(line))
                Console.WriteLine(line.TrimEnd());
        }
    }
}