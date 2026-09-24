using System;
using HolmiumOS.Shell;

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

            try
            {
                FileSystemManager.CopyFile(copyArgs[0], copyArgs[1]);

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