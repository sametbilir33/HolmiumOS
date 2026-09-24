using System;

namespace HolmiumOS.Commands.System
{
    public class Date : ICommand
    {
        public string Name => "date";
        public string Description => "Mevcut tarihi ve saati gosterir";
        public string Usage => "date";

        public void Execute(string args)
        {
            DateTime now = DateTime.Now;

string formatted =
    $"{now.Year:D4}-{now.Month:D2}-{now.Day:D2} " +
    $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(formatted);
            Console.ResetColor();
        }
    }
}