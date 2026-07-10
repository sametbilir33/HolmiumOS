using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.UserSystem
{
    public class Whoami : ICommand
    {
        public string Name => "whoami";
        public string Description => "Mevcut oturumdaki kullaniciyi gosterir";
        public string Usage => "whoami";

        public void Execute(string args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            if (!UserManager.IsLoggedIn)
                Console.WriteLine("No user logged in.");
            else
                Console.WriteLine(UserManager.CurrentUser);

            Console.ResetColor();
        }
    }
}