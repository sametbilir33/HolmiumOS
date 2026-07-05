using HolmiumOS.Shell;
using System;
using System.IO;

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

            string path = Path.IsPathRooted(args)
                ? args
                : Path.Combine(FileSystemManager.CurrentDirectory, args);

            try
            {
                if (Directory.Exists(path) || File.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hata: Ayni adda dosya veya klasor zaten var.");
                    return;
                }

                Directory.CreateDirectory(path);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Klasor olusturuldu: {path}");
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