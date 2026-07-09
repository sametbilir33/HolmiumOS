using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.FileSystem
{
    public class Cat : ICommand
    {
        public string Name => "cat";
        public string Description => "Dosya icerigini gosterir";
        public string Usage => "cat <dosya>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(FileSystemManager.ReadFile(args));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}