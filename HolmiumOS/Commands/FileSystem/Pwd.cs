using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.FileSystem
{
    public class Pwd : ICommand
    {
        public string Name => "pwd";
        public string Description => "Aktif dizini gosterir";
        public string Usage => "pwd";

        public void Execute(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(FileSystemManager.CurrentDirectory);
            Console.ResetColor();
        }
    }
}