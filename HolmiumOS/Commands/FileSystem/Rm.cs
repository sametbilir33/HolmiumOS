using HolmiumOS.Shell;
using System;

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

            try
            {
                FileSystemManager.DeleteFile(args);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dosya silindi.");
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