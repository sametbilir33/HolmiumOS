using HolmiumOS.Shell;
using System;
using System.IO;

namespace HolmiumOS.Commands.FileSystem
{
    public class Touch : ICommand
    {
        public string Name => "touch";
        public string Description => "Yeni bir dosya olusturur";
        public string Usage => "touch <dosya>";

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
                if (File.Exists(path) || Directory.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hata: Ayni adda dosya veya klasor zaten var.");
                    return;
                }

                using (File.Create(path))
                {
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Dosya olusturuldu: {path}");
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