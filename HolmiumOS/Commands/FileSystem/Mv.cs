using HolmiumOS.Shell;
using System;
using System.IO;

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

            string source = Path.IsPathRooted(moveArgs[0])
                ? moveArgs[0]
                : Path.Combine(FileSystemManager.CurrentDirectory, moveArgs[0]);

            string destination = Path.IsPathRooted(moveArgs[1])
                ? moveArgs[1]
                : Path.Combine(FileSystemManager.CurrentDirectory, moveArgs[1]);

            try
            {
                if (!File.Exists(source))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: Kaynak dosya bulunamadi: {source}");
                    return;
                }

                File.Copy(source, destination, true);
                File.Delete(source);

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