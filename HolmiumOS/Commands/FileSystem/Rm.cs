using HolmiumOS.Shell;
using System;
using System.IO;

namespace HolmiumOS.Commands.FileSystem
{
    public class Del : ICommand
    {
        public string Name => "rm";
        public string Description => "Dosyayi siler";
        public string Usage => "rm <dosya>";

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
                if (!File.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Dosya bulunamadi.");
                    return;
                }

                File.Delete(path);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Dosya silindi: {path}");
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