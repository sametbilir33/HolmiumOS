using HolmiumOS.Shell;
using System;
using System.IO;

namespace HolmiumOS.Commands.FileSystem
{
    public class ls : ICommand
    {
        public string Name => "ls";
        public string Description => "Suanki dizindeki klasor ve dosyalari listeler";
        public string Usage => "ls [dizin]";

        public void Execute(string args)
        {
            string directory;

            if (string.IsNullOrWhiteSpace(args))
            {
                directory = FileSystemManager.CurrentDirectory;
            }
            else if (Path.IsPathRooted(args))
            {
                directory = args;
            }
            else
            {
                directory = Path.Combine(FileSystemManager.CurrentDirectory, args);
            }

            try
            {
                if (!Directory.Exists(directory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Dizin bulunamadi.");
                    return;
                }

                string[] directories = Directory.GetDirectories(directory);
                string[] files = Directory.GetFiles(directory);

                if (directories.Length == 0 && files.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Dizin bos.");
                    return;
                }

                foreach (string dir in directories)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"<DIR> {Path.GetFileName(dir)}");
                }

                foreach (string file in files)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"      {Path.GetFileName(file)}");
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