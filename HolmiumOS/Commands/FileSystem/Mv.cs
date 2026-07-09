using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.FileSystem
{
    public class Mv : ICommand
    {
        public string Name => "mv";
        public string Description => "Dosyayi tasir";
        public string Usage => "mv <kaynak> <hedef>";

        public void Execute(string args)
        {
            string[] moveArgs = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (moveArgs.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            try
            {
                FileSystemManager.MoveFile(moveArgs[0], moveArgs[1]);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dosya basariyla tasindi.");
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