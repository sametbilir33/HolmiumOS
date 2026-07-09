using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.FileSystem
{
    public class Mkdir : ICommand
    {
        public string Name => "mkdir";
        public string Description => "Klasor olusturur";
        public string Usage => "mkdir <klasor>";

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
                FileSystemManager.CreateDirectory(args);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Klasor olusturuldu.");
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