using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.System
{
    public class Clear : ICommand
    {
        public string Name => "clear";
        public string Description => "Ekrani temizler";
        public string Usage => "clear";

        public void Execute(string args)
        {
            if (TerminalWriter.Current != null)
            {
                TerminalWriter.ClearCurrent();
                return;
            }

            Console.Clear();
        }
    }
}