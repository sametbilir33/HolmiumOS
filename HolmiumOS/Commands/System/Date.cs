using System;
using HolmiumOS.Commands;
using Cosmos.HAL;

namespace HolmiumOS.Commands.System
{
    public class Date : ICommand
    {
        public string Name => "date";
        public string Description => "Mevcut tarihi ve saati gosterir";
        public string Usage => "date";

        public void Execute(string args)
        {
            string formatted = $"{RTC.Year:D4}-{RTC.Month:D2}-{RTC.DayOfTheMonth:D2} " +
                               $"{RTC.Hour:D2}:{RTC.Minute:D2}:{RTC.Second:D2}";

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(formatted);
            Console.ResetColor();
        }
    }
}