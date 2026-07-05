using Cosmos.System.FileSystem.VFS;
using HolmiumOS.Shell;
using System;
using System.IO;

namespace HolmiumOS.Commands.FileSystem
{
    public class Cd : ICommand
    {
        public string Name => "cd";
        public string Description => "Dizin degistirir";
        public string Usage => "cd <klasor>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            string target = args.Trim();
            string currentDir = Shell.FileSystemManager.CurrentDirectory;
            string newDir;

            if (target == "..")
            {
                if (currentDir == @"0:\")
                {
                    newDir = currentDir;
                }
                else
                {
                    string trimmed = currentDir.TrimEnd('\\');
                    int index = trimmed.LastIndexOf('\\');

                    if (index > 2)
                        newDir = trimmed.Substring(0, index + 1);
                    else
                        newDir = @"0:\";
                }
            }
            else if (Path.IsPathRooted(target))
            {
                newDir = target;
            }
            else
            {
                newDir = Path.Combine(currentDir, target);
            }

            if (!newDir.EndsWith("\\"))
                newDir += "\\";

            try
            {
                if (VFSManager.DirectoryExists(newDir))
                {
                    Shell.FileSystemManager.CurrentDirectory = newDir;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Dizin degisti: {newDir}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: '{target}' dizini bulunamadi.");
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