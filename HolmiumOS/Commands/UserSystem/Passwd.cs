using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.UserSystem
{
    public class Passwd : ICommand
    {
        public string Name => "passwd";
        public string Description => "Kullanici parolasini degistirir";
        public string Usage => "passwd [kullanici]";

        public void Execute(string args)
        {
            if (!UserManager.IsLoggedIn)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oturum acmaniz gerekiyor.");
                Console.ResetColor();
                return;
            }

            string username = UserManager.CurrentUser;

            if (!string.IsNullOrWhiteSpace(args))
            {
                if (!UserManager.IsRoot)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bu islem root yetkisi gerektirir.");
                    Console.ResetColor();
                    return;
                }

                username = args.Trim().ToLower();

                if (!UserManager.UserExists(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Kullanici bulunamadi.");
                    Console.ResetColor();
                    return;
                }
            }
            else if (!UserManager.IsRoot)
            {
                Console.Write("Current password: ");
                string current = PasswordReader.ReadPassword();

                if (!UserManager.VerifyPassword(username, current))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hatali parola.");
                    Console.ResetColor();
                    return;
                }
            }

            Console.Write("New password: ");
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

            if (UserManager.ChangePassword(username, pass1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Parola degistirildi.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Parola degistirilemedi.");
            }

            Console.ResetColor();
        }
    }
}