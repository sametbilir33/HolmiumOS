using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.FileSystem
{
    public class Cd : ICommand
    {
        public string Name => "cd";
        public string Description => "Dizin degistirir";
        public string Usage => "cd <klasor>";

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
                bool success = FileSystemManager.ChangeDirectory(args);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Dizin degisti: {FileSystemManager.GetDisplayPath()}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: '{args}' dizini bulunamadi.");
                }
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