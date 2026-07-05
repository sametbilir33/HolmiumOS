using HolmiumOS.Shell;
using System;
using System.IO;

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

                string content = File.ReadAllText(path);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(content);
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