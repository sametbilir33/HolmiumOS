using System;
using System.IO;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.FileSystem
{
    public class Stat : ICommand
    {
        public string Name => "stat";
        public string Description => "Dosya veya klasorun bilgilerini gosterir";
        public string Usage => "stat <dosya|klasor>";

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
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Dosya Bilgileri");
                    Console.WriteLine("----------------------");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Ad      : {fi.Name}");
                    Console.WriteLine($"Tur     : Dosya");
                    Console.WriteLine($"Uzanti  : {fi.Extension}");
                    Console.WriteLine($"Boyut   : {fi.Length} byte");
                    Console.WriteLine($"Tam Yol : {fi.FullName}");
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Klasor Bilgileri");
                    Console.WriteLine("----------------------");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Ad      : {di.Name}");
                    Console.WriteLine($"Tur     : Klasor");
                    Console.WriteLine($"Tam Yol : {di.FullName}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Dosya veya klasor bulunamadi.");
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