using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.UserSystem
{
    public class UserAdd : ICommand
    {
        public string Name => "useradd";
        public string Description => "Yeni bir kullanici olusturur";
        public string Usage => "useradd <kullanici>";

        public void Execute(string args)
        {
            if (!UserManager.IsRoot)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bu islem root yetkisi gerektirir.");
                Console.ResetColor();
                return;
            }

            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            string username = args.Trim().ToLower();

            if (!UserManager.IsValidUsername(username, out string error))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
                return;
            }

            if (UserManager.UserExists(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bu kullanici zaten mevcut.");
                Console.ResetColor();
                return;
            }

            Console.Write("Password: ");
            string pass1 = PasswordReader.ReadPassword();

            Console.Write("Confirm password: ");
            string pass2 = PasswordReader.ReadPassword();

            if (pass1 != pass2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Parolalar eslesmiyor.");
                Console.ResetColor();
                return;
            }

            if (UserManager.CreateUser(username, pass1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"'{username}' kullanicisi olusturuldu.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kullanici olusturulamadi.");
            }

            Console.ResetColor();
        }
    }
}