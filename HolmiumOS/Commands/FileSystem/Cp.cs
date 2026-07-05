using HolmiumOS.Shell;
using System;
using System.IO;

namespace HolmiumOS.Commands.FileSystem
{
    public class Cp : ICommand
    {
        public string Name => "cp";
        public string Description => "Dosyayi kopyalar";
        public string Usage => "cp <kaynak> <hedef>";

        public void Execute(string args)
        {
            string[] copyArgs = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (copyArgs.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            string source = Path.IsPathRooted(copyArgs[0])
                ? copyArgs[0]
                : Path.Combine(FileSystemManager.CurrentDirectory, copyArgs[0]);

            string destination = Path.IsPathRooted(copyArgs[1])
                ? copyArgs[1]
                : Path.Combine(FileSystemManager.CurrentDirectory, copyArgs[1]);

            try
            {
                if (!File.Exists(source))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: Kaynak dosya bulunamadi: {source}");
                    return;
                }

                File.Copy(source, destination, true);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dosya basariyla kopyalandi.");
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