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
            try
            {
                string[] directories = FileSystemManager.GetDirectories(args);
                string[] files = FileSystemManager.GetFiles(args);

                if (directories.Length == 0 && files.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Dizin bos.");
                    return;
                }

                foreach (string dir in directories)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Path.GetFileName(dir));
                }

                foreach (string file in files)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Path.GetFileName(file));
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