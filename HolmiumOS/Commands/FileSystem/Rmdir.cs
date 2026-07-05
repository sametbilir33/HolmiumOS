using HolmiumOS.Shell;
using System;
using System.IO;

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
                if (args == "*")
                {
                    foreach (string dir in Directory.GetDirectories(FileSystemManager.CurrentDirectory))
                    {
                        Directory.Delete(dir, true);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Klasor silindi: {Path.GetFileName(dir)}");
                    }
                }
                else
                {
                    string path = Path.IsPathRooted(args)
                        ? args
                        : Path.Combine(FileSystemManager.CurrentDirectory, args);

                    if (!Directory.Exists(path))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Klasor bulunamadi.");
                        return;
                    }

                    Directory.Delete(path, true);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Klasor silindi: {path}");
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