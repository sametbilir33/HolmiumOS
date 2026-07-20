using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.UserSystem
{
    public class Logout : ICommand
    {
        public string Name => "logout";
        public string Description => "Oturumu kapatir";
        public string Usage => "logout";

        public void Execute(string args)
        {
            if (!UserManager.IsLoggedIn)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No user logged in.");
                Console.ResetColor();
                return;
            }

            string username = UserManager.CurrentUser;

            UserManager.Logout();

            FileSystemManager.CurrentDirectory = @"0:\";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{username} logged out.");

            Console.ResetColor();
        }
    }
}