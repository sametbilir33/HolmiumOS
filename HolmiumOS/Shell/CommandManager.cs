using System;
using System.Collections.Generic;
using HolmiumOS.Commands;
using HolmiumOS.Commands.FileSystem;
using HolmiumOS.Commands.Fun;
using HolmiumOS.Commands.Math;
using HolmiumOS.Commands.System;
using HolmiumOS.Commands.Tools;
using HolmiumOS.Commands.Executable;
using HolmiumOS.Commands.UserSystem;
using HolmiumOS.Commands.Sound;
using HolmiumOS.Commands.Crypto;
using HolmiumOS.Commands.Network;

namespace HolmiumOS.Shell
{
    public static class CommandManager
    {
        private static List<ICommand> commands = new List<ICommand>();

        public static IEnumerable<ICommand> Commands => commands;

        public static void RegisterCommands()
        {
            commands.Add(new Help());

            commands.Add(new Clear());
            commands.Add(new Date());
            commands.Add(new Echo());
            commands.Add(new Fetch());
            commands.Add(new Pause());
            commands.Add(new Reboot());
            commands.Add(new Shutdown());
            commands.Add(new Reset());
            commands.Add(new Ember());
            commands.Add(new HolmiumOS.Commands.System.PCI());

            commands.Add(new Calc());
            commands.Add(new EvenOdd());
            commands.Add(new Fact());
            commands.Add(new Gcd());
            commands.Add(new IsPrime());
            commands.Add(new Lcm());
            commands.Add(new Mult());
            commands.Add(new Pow());
            commands.Add(new Sqrt());

            commands.Add(new Cowsay());
            commands.Add(new BadApple());

            commands.Add(new Cd());
            commands.Add(new Cp());
            commands.Add(new Touch());
            commands.Add(new Del());
            commands.Add(new ls());
            commands.Add(new Disks());
            commands.Add(new DiskUsage());
            commands.Add(new Mkdir());
            commands.Add(new Mv());
            commands.Add(new Pwd());
            commands.Add(new Rmdir());
            commands.Add(new Stat());
            commands.Add(new Cat());
            commands.Add(new Run());
            commands.Add(new Miv());

            commands.Add(new Rand());
            commands.Add(new LoadKeys());

            commands.Add(new Login());
            commands.Add(new Logout());
            commands.Add(new Passwd());
            commands.Add(new Whoami());
            commands.Add(new Su());
            commands.Add(new Sudo());
            commands.Add(new UserAdd());
            commands.Add(new UserDel());

            commands.Add(new PlayMusic());
            commands.Add(new StopMusic());
            commands.Add(new MusicStatus());
            commands.Add(new PauseMusic());

            commands.Add(new Sha256());
            commands.Add(new VerifySha256());
            commands.Add(new Md5());
            commands.Add(new VeridyMD5());

            commands.Add(new Wget());
            commands.Add(new Curl());
        }

        public static void ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string[] parts = input.Split(' ', 2);
            string commandName = parts[0].ToLower();
            string args = parts.Length > 1 ? parts[1] : "";

            foreach (var cmd in commands)
            {
                if (cmd.Name == commandName)
                {
                    cmd.Execute(args);
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Bilinmeyen komut.");
            Console.ResetColor();
        }
    }
}