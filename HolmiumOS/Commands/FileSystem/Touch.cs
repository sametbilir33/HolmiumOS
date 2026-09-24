using System;
using HolmiumOS.Shell;

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

            try
            {
                FileSystemManager.CreateFile(args);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Dosya olusturuldu: {args}");
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