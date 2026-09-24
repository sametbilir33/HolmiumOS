using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.FileSystem
{
    public class Rmdir : ICommand
    {
        public string Name => "rmdir";
        public string Description => "Klasor siler";
        public string Usage => "rmdir <klasor>";

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
                FileSystemManager.DeleteDirectory(args);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Klasor silindi: {args}");
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