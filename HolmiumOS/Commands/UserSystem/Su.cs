using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.UserSystem
{
    public class Su : ICommand
    {
        public string Name => "su";
        public string Description => "Baska bir kullaniciya gecis yapar";
        public string Usage => "su [kullanici]";

        public void Execute(string args)
        {
            string username = string.IsNullOrWhiteSpace(args)
                ? "root"
                : args.Trim().ToLower();

            if (!UserManager.UserExists(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kullanici bulunamadi.");
                Console.ResetColor();
                return;
            }

            Console.Write("Password: ");
            string password = PasswordReader.ReadPassword();

            if (!UserManager.Login(username, password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hatali parola.");
                Console.ResetColor();
                return;
            }

            FileSystemManager.CurrentDirectory = UserManager.HomeDirectory;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Simdi '{username}' olarak oturum acildi.");
            Console.ResetColor();
        }
    }
}