using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.UserSystem
{
    public class Sudo : ICommand
    {
        public string Name => "sudo";
        public string Description => "Bir komutu root yetkisi ile calistirir";
        public string Usage => "sudo <komut>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Kullanim: {Usage}");
                Console.ResetColor();
                return;
            }

            if (UserManager.IsRoot)
            {
                CommandManager.ExecuteCommand(args);
                return;
            }

            Console.Write("Password: ");
            string password = PasswordReader.ReadPassword();

            if (!UserManager.VerifyPassword("root", password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hatali parola.");
                Console.ResetColor();
                return;
            }

            try
            {
                PermissionManager.IsElevated = true;
                CommandManager.ExecuteCommand(args);
            }
            finally
            {
                PermissionManager.IsElevated = false;
            }
        }
    }
}