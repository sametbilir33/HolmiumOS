using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.UserSystem
{
    public class Login : ICommand
    {
        public string Name => "login";
        public string Description => "Bir kullanici ile oturum acar";
        public string Usage => "login";

        public void Execute(string args)
        {
            if (UserManager.IsLoggedIn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Zaten '{UserManager.CurrentUser}' olarak giris yapilmis.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Username: ");
            string username = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Password: ");
            string password = PasswordReader.ReadPassword();

            if (UserManager.Login(username, password))
            {
                FileSystemManager.CurrentDirectory = UserManager.HomeDirectory;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Hosgeldin, {UserManager.CurrentUser}.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hatali kullanici adi veya parola.");
            }

            Console.ResetColor();
        }
    }
}