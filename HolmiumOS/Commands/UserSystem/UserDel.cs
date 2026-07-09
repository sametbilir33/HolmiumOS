using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.UserSystem
{
    public class UserDel : ICommand
    {
        public string Name => "userdel";
        public string Description => "Bir kullaniciyi siler";
        public string Usage => "userdel <kullanici>";

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

            if (!UserManager.UserExists(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kullanici bulunamadi.");
                Console.ResetColor();
                return;
            }

            if (username == "root")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Root kullanicisi silinemez.");
                Console.ResetColor();
                return;
            }

            if (username == UserManager.CurrentUser)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aktif oturumdaki kullanici silinemez.");
                Console.ResetColor();
                return;
            }

            if (UserManager.DeleteUser(username))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"'{username}' silindi.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kullanici silinemedi.");
            }

            Console.ResetColor();
        }
    }
}